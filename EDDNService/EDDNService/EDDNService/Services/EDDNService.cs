using System;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Configuration;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;

using Elmah;
using NetMQ;
using Newtonsoft.Json;

using EDDNService.Models;
using EliteTrading.Entities;

using System.Data.SqlClient;
using System.Data;
using System.Timers;
using Newtonsoft.Json.Linq;

namespace EDDNService.Services {
    public class EDDN : IDisposable {
        private static readonly Lazy<EDDN> _EDDNService = new Lazy<EDDN>(() => new EDDN());

        private static double _version;
        private static string _schemaRef;
        private static string _EDDNConnection;
        private static bool _listen, _connected;
        private static ApplicationDbContext _db;
        private static ReputationService _rep;
        private static Task _listenToEDDN;
        private static System.Timers.Timer _aTimer;
        private static Dictionary<string,string> _nameCorrectionMappingDictionary { get; set; }


        public static bool Connected { get { return _connected; } }

        private EDDN() {
            _version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
            _schemaRef = ConfigurationManager.AppSettings["SchemaRef"];
            _EDDNConnection = ConfigurationManager.AppSettings["EDDNConnection"];

            _nameCorrectionMappingDictionary = new Dictionary<string, string>();
            _nameCorrectionMappingDictionary.Add("Agricultural Medicines", "Agri-Medicines");
            _nameCorrectionMappingDictionary.Add("Ai Relics", "AI Relics");
            _nameCorrectionMappingDictionary.Add("Animalmeat", "Animal Meat");
            _nameCorrectionMappingDictionary.Add("Atmospheric Extractors", "Atmospheric Processors");
            _nameCorrectionMappingDictionary.Add("Auto Fabricators", "Auto-Fabricators");
            _nameCorrectionMappingDictionary.Add("Basic Narcotics", "Narcotics");
            _nameCorrectionMappingDictionary.Add("Bio Reducing Lichen", "Bioreducing Lichen");
            _nameCorrectionMappingDictionary.Add("C M M Composite", "CMM Composite");
            _nameCorrectionMappingDictionary.Add("Comercial Samples", "Commercial Samples");
            _nameCorrectionMappingDictionary.Add("Diagnostic Sensor", "Hardware Diagnostic Sensor");
            _nameCorrectionMappingDictionary.Add("Drones", "Limpet");
            _nameCorrectionMappingDictionary.Add("Encripted Data Storage", "Encrypted Data Storage");
            _nameCorrectionMappingDictionary.Add("H N Shock Mount", "HN Shock Mount");
            _nameCorrectionMappingDictionary.Add("Hafnium178", "Hafnium 178");
            _nameCorrectionMappingDictionary.Add("Hazardous Environment Suits", "H.E. Suits");
            _nameCorrectionMappingDictionary.Add("Heliostatic Furnaces", "Microbial Furnaces");
            _nameCorrectionMappingDictionary.Add("Ion Distributor", "Ion Distributor");
            _nameCorrectionMappingDictionary.Add("Low Temperature Diamond", "Low Temperature Diamonds");
            _nameCorrectionMappingDictionary.Add("Marine Supplies", "Marine Equipment");
            _nameCorrectionMappingDictionary.Add("Meta Alloys", "Meta-Alloys");
            _nameCorrectionMappingDictionary.Add("Methanol Monohydrate Crystals", "Methanol Monohydrate");
            _nameCorrectionMappingDictionary.Add("Mu Tom Imager", "Muon Imager");
            _nameCorrectionMappingDictionary.Add("Non Lethal Weapons", "Non-Lethal Weapons");
            _nameCorrectionMappingDictionary.Add("Power Grid Assembly", "Energy Grid Assembly");
            _nameCorrectionMappingDictionary.Add("Power Transfer Conduits", "Power Transfer Bus");
            _nameCorrectionMappingDictionary.Add("S A P8 Core Container", "SAP 8 Core Container");
            _nameCorrectionMappingDictionary.Add("Skimer Components", "Skimmer Components");
            _nameCorrectionMappingDictionary.Add("Terrain Enrichment Systems", "Land Enrichment Systems");
            _nameCorrectionMappingDictionary.Add("Trinkets Of Fortune", "Trinkets Of Hidden Fortune");
            _nameCorrectionMappingDictionary.Add("Unknown Artifact", "Unknown Artefact");
            _nameCorrectionMappingDictionary.Add("Unknown Artifact2", "Unknown Probe");
            _nameCorrectionMappingDictionary.Add("U S S Cargo Ancient Artefact", "Ancient Artefact");
            _nameCorrectionMappingDictionary.Add("U S S Cargo Experimental Chemicals", "Experimental Chemicals");
            _nameCorrectionMappingDictionary.Add("U S S Cargo Military Plans", "Military Plans");
            _nameCorrectionMappingDictionary.Add("U S S Cargo Prototype Tech", "Prototype Tech");
            _nameCorrectionMappingDictionary.Add("U S S Cargo Rebel Transmissions", "Rebel Transmissions");
            _nameCorrectionMappingDictionary.Add("U S S Cargo Technical Blueprints", "Technical Blueprints");
            _nameCorrectionMappingDictionary.Add("U S S Cargo Trade Data", "Trade Data");
            _nameCorrectionMappingDictionary.Add("Wreckage Components", "Salvageable Wreckage");

            _db = new ApplicationDbContext();
            _rep = new ReputationService();

            _aTimer = new System.Timers.Timer();
            _aTimer.Elapsed += new ElapsedEventHandler(CheckThreadTimed);
            _aTimer.Interval = 10000;
            _aTimer.Enabled = true;
            _aTimer.Stop();

            StartThread();

        }

        public static EDDN Instance {
            get {
                return _EDDNService.Value;
            }
        }

        private static void StartThread() {
            System.Diagnostics.Debug.WriteLine("Thread Create");
            _listen = true;
            _connected = false;
            _listenToEDDN = Task.Run(() => ListenToEDDN(_version, _schemaRef, _EDDNConnection, ref _listen, ref _connected));
            Task continuation = _listenToEDDN.ContinueWith(antecedent => ThreadComplete());
        }

        private static void ThreadComplete() {
            if (_listen) {
                // clean up the thread and start a new one
                _listenToEDDN = null;
                StartThread();
                System.Diagnostics.Debug.WriteLine("ThreadComplete restarting");
            } else if (_listenToEDDN.IsCompleted && _listen == false) {
                _listenToEDDN.Dispose();
                _aTimer.Start();
                System.Diagnostics.Debug.WriteLine("ThreadComplete starting timer");
            }
        }

        private static void CheckThreadTimed(object source, ElapsedEventArgs e) {
            System.Diagnostics.Debug.WriteLine("CheckThreadTimed");
            if (_listen) {
                _aTimer.Stop();
                StartThread();
            }
        }

        public static void StartListening() {
            _listen = true;
        }

        public static void StopListening() {
            _listen = false;
        }

        public static string GetThreadStatus() {
            if (_listenToEDDN.IsCompleted == true)
                return "Complete";

            switch (_listenToEDDN.Status) {
                case TaskStatus.Canceled:
                    return "Canceled";
                case TaskStatus.Created:
                    return "Created";
                case TaskStatus.Faulted:
                    return "Faulted";
                case TaskStatus.RanToCompletion:
                    return "RanToCompletion";
                case TaskStatus.Running:
                    return "Running";
                case TaskStatus.WaitingForActivation:
                    return "WaitingForActivation";
                case TaskStatus.WaitingForChildrenToComplete:
                    return "WaitingForChildrenToComplete";
                case TaskStatus.WaitingToRun:
                    return "WaitingToRun";
                default:
                    return "";
            }
        }

        private static void ListenToEDDN(double version, string schemaRef, string EDDNConnection, ref bool listen, ref bool connected) {
            using (NetMQContext ctx = NetMQContext.Create()) {


                // Attempt to connect
                using (var subscriber = Connect(ctx, EDDNConnection, ref _db, ref connected)) {
                    // Get the commodities
                    List<Commodity> commodities = _db.Commodities.ToList();

                    // Loop until app close 
                    while (listen) {

                        // read data from the stream
                        var byteArray = subscriber.ReceiveFrameBytes();

                        // Decompress the byte array
                        var decompressedFileStream = new MemoryStream();
                        using (decompressedFileStream) {
                            Stream stream = new MemoryStream(byteArray);

                            // Don't forget to ignore the first two bytes of the stream (!)
                            stream.ReadByte();
                            stream.ReadByte();
                            using (var decompressionStream = new DeflateStream(stream, CompressionMode.Decompress)) {
                                decompressionStream.CopyTo(decompressedFileStream);
                            }

                            decompressedFileStream.Position = 0;
                            var sr = new StreamReader(decompressedFileStream);
                            var myStr = sr.ReadToEnd();

                            JObject obj = JObject.Parse(myStr);
                            string readSchemaRef = (string)obj["$schemaRef"];

                            // Check the data is commodities, reject if not
                            if (readSchemaRef == schemaRef) {

                                // deserialise the json into objects
                                data d = JsonConvert.DeserializeObject<data>(myStr);


                                // Check the database
                                string commanderName = d.Header.UploaderId;
                                int systemId = 0, stationId = 0;
                                string userName = string.Empty;
                                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                                SqlCommand cmd = new SqlCommand("EDDNSearch", con);
                                cmd.CommandType = CommandType.StoredProcedure;

                                SqlParameter param;
                                param = cmd.Parameters.Add("@SystemName", SqlDbType.NVarChar, 50);
                                param.Value = d.Message.SystemName;

                                param = cmd.Parameters.Add("@StationName", SqlDbType.NVarChar, 60);
                                param.Value = d.Message.StationName;

                                param = cmd.Parameters.Add("@CommanderName", SqlDbType.NVarChar, 50);
                                param.Value = commanderName;

                                param = cmd.Parameters.Add("@SystemId", SqlDbType.Int);
                                param.Direction = ParameterDirection.Output;
                                param.IsNullable = true;

                                param = cmd.Parameters.Add("@StationId", SqlDbType.Int);
                                param.Direction = ParameterDirection.Output;

                                param = cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 256);
                                param.Direction = ParameterDirection.Output;

                                try {
                                    // Execute the command.
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    if (cmd.Parameters["@SystemId"].Value != null && DBNull.Value != cmd.Parameters["@SystemId"].Value)
                                        systemId = Convert.ToInt32(cmd.Parameters["@SystemId"].Value);

                                    if (cmd.Parameters["@StationId"].Value != null && DBNull.Value != cmd.Parameters["@StationId"].Value)
                                        stationId = Convert.ToInt32(cmd.Parameters["@StationId"].Value);

                                    userName = cmd.Parameters["@UserName"].Value.ToString();

                                } catch (Exception ex) {
                                    ErrorLog.GetDefault(null).Log(new Error(ex));
                                } finally {
                                    con.Close();
                                }


                                // Do we have the system
                                if (systemId > 0) {
                                    // Do we have the station
                                    if (stationId > 0) {


                                        // Get the stations existing commodities
                                        List<StationCommodity> list = _db.StationCommodities.Include("Commodity").Where(m => m.StationId == stationId).ToList();
                                        foreach (commodity sc in d.Message.Commodities) {
                                            // Check if the commodity name needs mapped

                                            if (_nameCorrectionMappingDictionary.ContainsKey(sc.Name)) {
                                                sc.Name = _nameCorrectionMappingDictionary[sc.Name];
                                            }

                                            // Check we have this commodity in the DB
                                            Commodity commodity = commodities.Where(m => m.Name == sc.Name).FirstOrDefault();
                                                
                                            if (commodity != null) {
                                                // Try to find the StationCommodity in the list of commodities for this station
                                                StationCommodity item = list.Where(m => m.Commodity.Name == sc.Name).FirstOrDefault();
                                                    
                                                // Check if it exists
                                                if (item != null) { // Update
                                                    // See if the existing entry is newer
                                                    if (item.LastUpdate < d.Message.Timestamp.AddHours(-5)) {
                                                        // Its older so lets update it
                                                        item.LastUpdate = d.Message.Timestamp.AddHours(-5);
                                                        item.Buy = sc.BuyPrice;
                                                        item.Sell = sc.SellPrice;
                                                        item.UpdatedBy = commanderName;
                                                        item.Version = version;

                                                        if(sc.SupplyLevel != null) {
                                                            item.Supply = ((SupplyDemandLevel)sc.SupplyLevel).ToString();
                                                        } else {
                                                            item.Supply = SupplyDemandLevel.None.ToString();
                                                        }
                                                        item.SupplyAmount = sc.Supply;

                                                        if (sc.DemandLevel != null) {
                                                            item.Demand = ((SupplyDemandLevel)sc.DemandLevel).ToString();
                                                        } else {
                                                            item.Demand = SupplyDemandLevel.None.ToString();
                                                        }
                                                        item.DemandAmount = sc.Demand;

                                                        
                                                        _db.Entry(item).State = EntityState.Modified;
                                                    }
                                                } else { // Create
                                                    item = new StationCommodity();
                                                    item.StationId = stationId;
                                                    item.CommodityId = commodity.Id;
                                                    item.LastUpdate = d.Message.Timestamp.AddHours(-5);
                                                    item.Buy = sc.BuyPrice;
                                                    item.Sell = sc.SellPrice;
                                                    item.UpdatedBy = commanderName;
                                                    item.Version = version;
                                                    item.Supply = ((SupplyDemandLevel)sc.SupplyLevel).ToString();
                                                    item.SupplyAmount = sc.Supply;
                                                    item.Demand = ((SupplyDemandLevel)sc.DemandLevel).ToString();
                                                    item.DemandAmount = sc.Demand;
                                                    _db.StationCommodities.Add(item);
                                                }
                                            }
                                        }

                                        // delete any that weren't updated
                                        var toDelete = list.Where(m => m.UpdatedBy != commanderName).ToList();
                                        _db.StationCommodities.RemoveRange(toDelete);
                                        _db.SaveChanges();

                                        if (userName != string.Empty) {
                                            _rep.AddRep(userName, ActionRep.ImportData);
                                            System.Diagnostics.Debug.WriteLine("Rep added to " + commanderName);
                                        }

                                        Log(ref _db, "Imported", d.Message.SystemName + " (" + d.Message.StationName + ")");
                                        System.Diagnostics.Debug.WriteLine(d.Message.SystemName + " (" + d.Message.StationName + ") " + commanderName);


                                    } else {
                                        Log(ref _db, "Missing Station", d.Message.SystemName + " (" + d.Message.StationName + ")", systemId);
                                    }
                                } else {
                                    Log(ref _db, "Missing System", d.Message.SystemName);
                                }
                                
                            }
                            decompressedFileStream.Close();
                        }
                    }
                    Log(ref _db, "Conn", "Disconnecting");

                    subscriber.Disconnect(EDDNConnection);
                    connected = false;
                    subscriber.Dispose();

                }
            }
        }

        private static NetMQ.Sockets.SubscriberSocket Connect(NetMQContext ctx, string EDDNConnection, ref ApplicationDbContext db, ref bool connected) {
            var subscriber = ctx.CreateSubscriberSocket();

            while (!connected) {
                try {
                    // try to connect & subscribe
                    subscriber.Connect(EDDNConnection);
                    subscriber.Options.TcpKeepalive = true;
                    subscriber.Subscribe(string.Empty);
                    connected = true;

                    System.Diagnostics.Debug.WriteLine("Connected to EDDN");
                    Log(ref db, "Conn", "Connected");

                } catch (Exception ex) {
                    ErrorLog.GetDefault(null).Log(new Error(ex));

                    System.Diagnostics.Debug.WriteLine("Unable to connect to EDDN");
                    Log(ref db, "Conn", "No Connection");

                    // No connection, wait 1 minute
                    System.Threading.Thread.Sleep(60000);
                }
            }

            return subscriber;
        }


        private static void Log(ref ApplicationDbContext db, string action, string message, int systemId = 0) {
            EDDNLog log = new EDDNLog {
                Action = action,
                Message = message,
                TimeStamp = DateTime.Now,
                SystemId = systemId
            };
            db.EDDNLogs.Add(log);
            db.SaveChanges();
        }

        public void Dispose() {
            _db.Dispose();
            _listenToEDDN.Dispose();
        }
    }
}