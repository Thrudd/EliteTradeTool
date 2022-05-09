/******************************************/
/*          Global Variables              */
/******************************************/
var selectLists = {};
var adminSelectorLabels = [];
var adminSelectorMapped = {};
var stationData = {};
var commodityData = {};
var stationsData = {};
var commodities = {};

function capitaliseFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}







/******************************************/
/*         Display Functions              */
/******************************************/
function RefreshSystemData() {
    var systemStationsData = new Bloodhound({
        name: 'systemStationsData',
        limit: 15,
        remote: {
            url: '/Admin/SelectorData?query=%QUERY',
            filter: function (data) {
                adminSelectorLabels = [];
                adminSelectorMapped = {};
                $.each(data, function (i, item) {
                    if (item.Station != "") {
                        adminSelectorMapped[item.System + " (" + item.Station + ")"] = item;
                        adminSelectorLabels.push({ name: item.System + " (" + item.Station + ")" });
                    } else {
                        adminSelectorMapped[item.System] = item;
                        adminSelectorLabels.push({ name: item.System });
                    }
                });
                return adminSelectorLabels;
            }
        },
        datumTokenizer: function (d) {
            return Bloodhound.tokenizers.whitespace(d.val);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace
    });
    systemStationsData.initialize();
    $('#Selector').typeahead('destroy');
    $('#Selector').typeahead({
        hint: true,
        highlight: true,
        minLength: 2
    }, {
        name: 'systemStationsData',
        displayKey: 'name',
        source: systemStationsData.ttAdapter()
    }).bind('typeahead:selected', function (obj, datum) {
        ShowDetails(adminSelectorMapped[datum.name]);
    }).bind('typeahead:autocompleted', function (obj, datum) {
        ShowDetails(adminSelectorMapped[datum.name]);
    });

}




function ShowDetails(selected) {
    $("#AdminContentContainer").removeClass('hidden');
    $("#System").html('');
    $("#Stations").html('');
    $("#StationCommodities").html('');
    $("#Import").html('');
    
    $.ajax({
        url: '/Admin/System',
        type: 'GET',
        data: { id: selected.SystemId },
        success: function (data) {
            var template = $.templates("#systemTemplate");

            var htmlOutput = template.render(data);

            $("#System").html(htmlOutput);
        }
    });

    RenderStations(selected.SystemId);
    
}




function RenderStations(systemId) {
    $.ajax({
        url: '/Admin/SystemStations',
        type: 'GET',
        data: { id: systemId },
        success: function (data) {
            var template = $.templates("#systemStationsTemplate");
            stationData = {
                "CurrentMarketStationId": 0,
                "stations": data.Stations,
                "systemId": data.SystemId
            };
            var htmlOutput = template.render(stationData);

            $("#Stations").html(htmlOutput);
            $("#StationCommodities").html('');
            stationsData = data.Stations;
        }
    });
}




function GetStationCommodities(id) {
    stationData.CurrentMarketStationId = id;
    $.ajax({
        url: '/Admin/StationCommodities',
        type: 'GET',
        data: { id: id, categoryId: 0 },
        success: function (data) {
            commodityData = {
                "StationCommodities": data.StationCommodities,
                "StationName": data.StationName,
                "StationId": data.StationId,
                "CommodityCategories": selectLists.CommodityCategories
            };
            var template = $.templates("#stationCommoditiesTemplate");
            var htmlOutput = template.render(commodityData);
            $("#StationCommodities").html(htmlOutput);

            if (data.StationCommodities.length > 0) {
                $("#ConfirmManyBtn").css("display", "block");
            } else {
                $("#ConfirmManyBtn").css("display", "none");
            }


            $('#CommodityList tr').not("#StationCommodity0").on('blur', 'input', function (e) {
                var stationCommodityId = $(this).data('id');
                var action = $(this).data('action');
                var initalvalue = $(this).data('initalvalue');
                var value = $(this).val();

                if (value != initalvalue && value != '' ) {
                    $.ajax({
                        url: '/Admin/UpdateCommodity',
                        type: 'POST',
                        data: { StationCommodityId: stationCommodityId, Action: action, Value: value },
                        success: function (data) {
                            var target = $("#" + action + stationCommodityId);
                            target.popover('destroy');
                            target.addClass("updated").delay(2000).queue(function () {
                                $(this).removeClass("updated").dequeue();
                            });
                            $("#Updated" + data.StationCommodityId).prop("title", "Updated by CMDR " + data.UpdatedBy).html(data.LastUpdate).addClass('text-success').delay(2000).queue(function () {
                                $(this).removeClass("text-success").dequeue();
                            });
                            $("#" + capitaliseFirstLetter(action) + stationCommodityId).data('initalvalue', value);
                            if (data.RepResult != undefined) {
                                if (data.RepResult.RankUp) {
                                    NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
                                }
                            }
                        }, error: function (e, x, exception) {
                            var target = $("#" + action + stationCommodityId);
                            var _popover = target.popover({
                                trigger: "manual",
                                placement: "bottom",
                                content: exception,
                                template: "<div class=\"popover\"><div class=\"arrow\"></div><div class=\"popover-inner\"><div class=\"popover-content\"><p></p></div></div></div>"
                            });
                            // Bootstrap 3.x :      
                            var popover = target.attr('data-content', exception).data('bs.popover');
                            popover.setContent();
                            popover.$tip.addClass(popover.options.placement);
                            return $(target).popover("show");
                        }
                    });
                }
            });

            $('#CommodityList tr').not("#StationCommodity0").keyup(function (e) {
                var action = $(e.target).data("action");
                var id = $(e.target).data("id");
                var row = $(e.target).closest('tr');
                if (e.keyCode == 38) {
                   
                    var newRow = row.prev();
                    $("." + action, newRow).focus().select();
                    return false;
                }

                if (e.keyCode == 40) {
                    var newRow = row.next();
                    $("." + action, newRow).focus().select();
                    return false;
                }

                if (e.keyCode == 39 && action == "sell") {
                    $(".buy", row).focus().select();
                    return false;
                }

                if (e.keyCode == 37 && action == "buy") {
                    $(".sell", row).focus().select();
                    return false;
                }
            });

            $("#CategoryId").on("change", function () {
                var categoryId = $("#CategoryId").val();
                if (categoryId == 0) {
                    $("#AddOneCommodityBtn").css("display", "none");
                    $("#AddManyCommodityBtn span").html("Add All Missing Commodities");
                    $("#ConfirmManyBtn  .btnLabel").html(" Confirm All");
                } else {
                    $("#AddOneCommodityBtn").css("display", "inline-block");
                    var label = $("#CategoryId :selected").text();
                    $("#AddOneCommodityBtn span").html(label)
                    $("#AddManyCommodityBtn span").html("Add All In " + label);
                    $("#ConfirmManyBtn  .btnLabel").html(" Confirm These");
                }

                $.ajax({
                    url: '/Admin/StationCommodities',
                    type: 'GET',
                    data: { id: id, categoryId: categoryId },
                    success: function (data) {
                        RenderStationCommodityList(data);
                    }
                });

            });
        }
    });

}




function RenderStationCommodityList(data) {
    var template = $.templates("#commodityListTemplate");
    var htmlOutput = template.render(data.StationCommodities);
    $("#StationCommodities #CommodityList").html(htmlOutput);

    if (data.StationCommodities.length > 0) {
        $("#ConfirmManyBtn").css("display", "block");
    } else {
        $("#ConfirmManyBtn").css("display", "none");
    }
    $('#CommodityList tr').off();
    $('#CommodityList tr').not("#StationCommodity0").on('blur', 'input', function (e) {
        var stationCommodityId = $(this).data('id');
        var action = $(this).data('action');
        var initalvalue = $(this).data('initalvalue');
        var value = $(this).val();

        if (value != initalvalue && value != '') {

            $.ajax({
                url: '/Admin/UpdateCommodity',
                type: 'POST',
                data: { StationCommodityId: stationCommodityId, Action: action, Value: value },
                success: function (data) {
                    var target = $("#" + action + stationCommodityId);
                    target.popover('destroy');
                    target.addClass("updated").delay(2000).queue(function () {
                        $(this).removeClass("updated").dequeue();
                    });
                    $("#Updated" + data.StationCommodityId).prop("title", "Updated by CMDR " + data.UpdatedBy).html(data.LastUpdate).addClass('text-success').delay(2000).queue(function () {
                        $(this).removeClass("text-success").dequeue();
                    });

                }, error: function (e, x, exception) {
                    var target = $("#" + action + stationCommodityId);
                    var _popover = target.popover({
                        trigger: "manual",
                        placement: "bottom",
                        content: exception,
                        template: "<div class=\"popover\"><div class=\"arrow\"></div><div class=\"popover-inner\"><div class=\"popover-content\"><p></p></div></div></div>"
                    });
                    // Bootstrap 3.x :      
                    var popover = target.attr('data-content', exception).data('bs.popover');
                    popover.setContent();
                    popover.$tip.addClass(popover.options.placement);
                    return $(target).popover("show");
                }
            });
        }
    });

    $('#CommodityList tr').not("#StationCommodity0").keyup(function (e) {
        var action = $(e.target).data("action");
        var id = $(e.target).data("id");
        var row = $(e.target).closest('tr');
        if (e.keyCode == 38) {

            var newRow = row.prev();
            $("." + action, newRow).focus().select();
            return false;
        }

        if (e.keyCode == 40) {
            var newRow = row.next();
            $("." + action, newRow).focus().select();
            return false;
        }

        if (e.keyCode == 39 && action == "sell") {
            $(".buy", row).focus().select();
            return false;
        }

        if (e.keyCode == 37 && action == "buy") {
            $(".sell", row).focus().select();
            return false;
        }
    });
}



/******************************************/
/*          eliteOCR Import               */
/******************************************/
var currentImportPanel = "#ImportInstructions";
var importData = {};
var importResults = {
    CommoditiesNotFound: [],
    StationsNotFound: [],
    StationsFound: [],
    OutOfRange: []
};

function ShowImportPanel(name) {
    $(currentImportPanel + "Btn").removeClass("active");
    $(name + "Btn").addClass("active");
    $(currentImportPanel).addClass("hidden");
    $(name).removeClass("hidden");
    currentImportPanel = name;
}

function SetUpImport() {
    $.ajax({
        url: '/Admin/Commodities',
        type: 'GET',
        success: function (data) {
            for (i = 0; i < data.length; i++) {
                commodities[data[i].Name.toLowerCase()] = { "Id": data[i].Id, "Name": data[i].Name, "CategoryId": data[i].CategoryId, "CategoryName": data[i].CategoryName };
            }
        }
    });
}


function ParseImport() {
    $('input[type=file]').parse({
        config: {
            delimiter: ";",
            dynamicTyping: true,
            header: false,
            skipEmptyLines: true,
            complete: function (results, file) {
                importData.Commodities = results.data;
                importData.CommodityList = commodities;
                importResults = {
                    CommoditiesNotFound: [],
                    StationsNotFound: [],
                    StationsFound: [],
                    OutOfRange: []
                };
                // Ensure there is data to work with
                if (importData.Commodities.length > 0) {

                    importData.Stations = [];

                    // Loop through the data skip the first row as its a header
                    for (i = 1; i < importData.Commodities.length; i++) {
                        // Get the station name skip if no name
                        var stationName = importData.Commodities[i][1]; //Station
                        if (stationName == "")
                            continue;

                        // Check if the station already exists in the importData
                        var station = FindStation(stationName);

                        // If not make one
                        if (station == null) {
                            station = { "Id": 0, "StationName": stationName, "SystemName": importData.Commodities[i][0], "Commodities": [] }; // importData.Commodities[i][0] System
                            importData.Stations.push(station);
                        }

                        
                        var name, id, categoryName, categoryId;
                        var temp = importData.Commodities[i][2]; //Commodity
                        
                        // See if we can match the parsed row against a known commodity
                        if (typeof commodities[importData.Commodities[i][2].toLowerCase()] != 'undefined') {
                            name = commodities[importData.Commodities[i][2].toLowerCase()].Name; //Commodity
                            id = commodities[name.toLowerCase()].Id;

                            // Add the commodity to the collection
                            station.Commodities.push({
                                "CommodityId": id,
                                "Name": name,
                                "Sell": isNumber(importData.Commodities[i][3]) ? importData.Commodities[i][3] : 0, //importData.Commodities[i][3] Sell
                                "Buy": isNumber(importData.Commodities[i][4]) ? importData.Commodities[i][4] : 0, // importData.Commodities[i][4] Buy
                                "Date": importData.Commodities[i][9], // importData.Commodities[i][9] Date
                                "Supply": importData.Commodities[i][8], //Supply
                                "SupplyAmount": importData.Commodities[i][7], //SupplyAmount
                                "Demand": importData.Commodities[i][6], //Demand
                                "DemandAmount": importData.Commodities[i][5] //DemandAmount
                            });
                        } else {
                            importResults.CommoditiesNotFound.push({ StationName: station.Name, CommodityName: importData.Commodities[i][2] });
                        }
                        
                        
                    }
                    ParseComplete();
                } else {
                    // Data not found
                    ImportDataNotFound();
                }
            }
        }
    });
}


function FindStation(name) {
    for (var i = 0, len = importData.Stations.length; i < len; i++) {
        if (importData.Stations[i].StationName === name)
            return importData.Stations[i]; // Return as soon as the object is found
    }
    return null;
}


function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}


function ImportDataNotFound() {
    $("#ImportNotification").html("<p>No data parsed. Something must be wrong with the file format.</p>");
}


function ImportNoStationNamesFound() {
    $("#ImportNotification").html("<p>No station names found in data.</p>");
}


function ParseComplete() {

    var stations = [];
    for (i = 0; i < importData.Stations.length; i++) {
        stations.push({ Id: 0, StationName: importData.Stations[i].StationName, SystemName: importData.Stations[i].SystemName });
    }

    if (stations.length > 0) {
        // Query the server to check that the stations are in the database
        $.ajax({
            url: '/Admin/CheckStationNames',
            type: 'POST',
            data: JSON.stringify({ stations: stations }),
            contentType: "application/json",
            timeout: 10000,
            success: function (data) {
                var stationsToUpload = [];
                // Assign the station id to the 
                for (i = 0; i < data.length; i++) {
                    var station = FindStation(data[i].StationName);
                    station.Id = data[i].StationId;
                    if (station.Id != 0) {
                        for (j = 0; j < station.Commodities.length; j++) {
                            station.Commodities[j].StationId = station.Id;
                        }
                        stationsToUpload.push(station);
                        importResults.StationsFound.push({ StationName: station.SystemName + " (" + station.StationName+ ")" });
                    } else {
                        importResults.StationsNotFound.push({ StationName: station.SystemName + " (" + station.StationName + ")" });
                    }

                }
                if (stationsToUpload.length > 0) {
                    $.ajax({
                        url: '/Admin/ImportCommodities',
                        type: 'POST',
                        data: JSON.stringify({ data: stationsToUpload }),
                        contentType: "application/json",
                        success: function (data) {
                            importResults.Updated = parseInt(data.Updated);
                            importResults.Added = parseInt(data.Added);
                            importResults.Skipped = parseInt(data.Skipped);
                            importResults.Deleted = parseInt(data.Deleted);
                            importResults.OutOfRange = data.OutOfRange;
                            var template = $.templates("#ImportResultTemplate");
                            var htmlOutput = template.render(importResults);
                            $("#ImportForm").addClass("hidden");
                            $("#ImportNotification").addClass("hidden");
                            $("#ImportResult").removeClass("hidden");
                            $("#ImportResult").html(htmlOutput);
                            if (data.RepResult != undefined) {
                                if (data.RepResult.RankUp) {
                                    NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
                                }
                            }
                        }, error: ImportError
                    });
                } else {
                    var template = $.templates("#ImportResultTemplate");
                    var htmlOutput = template.render(importResults);
                    $("#ImportForm").addClass("hidden");
                    $("#ImportNotification").addClass("hidden");
                    $("#ImportResult").removeClass("hidden");
                    $("#ImportResult").html(htmlOutput);
                }
            }, error: ImportError
        });
    } else {
        ImportNoStationNamesFound();
    }
}

function ImportError(e, x, exception) {
    if (e.status === 0 || e.status === 502) {
        $("#ImportNotification").html("<div class='col-md-9 col-md-offset-3'><span style='color:#ef4023'>Error:</span> Unable to connect to server.</div>");
    } else if (e.status == 500) {
        $("#ImportNotification").html("<div class='col-md-9 col-md-offset-3'><span style='color:#ef4023'>Error:</span> Sorry commander an error has occured.<br />" + exception + "</div>");
    } else if (e.status == 400) {
        $("#ImportNotification").html("<div class='col-md-9 col-md-offset-3'><span style='color:#ef4023'>Invalid data:</span> " + exception + "</div>");
    }
    $("#ImportNotification").removeClass("hidden");
}

function ShowImportForm() {
    $("#ImportForm").removeClass("hidden");
    $("#ImportNotification").addClass("hidden");
    $("#ImportResult").addClass("hidden");
}



/******************************************/
/*               System                   */
/******************************************/
function ShowSystemEditor(id) {
    if (id == 0) {
        var systemData = {
            Id: 0,
            Name: "",
            AllegianceId: null,
            GovernmentId: null
        };
        var model = {
            System: systemData,
            Allegiances: selectLists.Allegiances,
            Governments: selectLists.Governments,
        };
        $("#System").html('');
        $("#Stations").html('');
        $("#StationCommodities").html('');
        RenderSystemEditor(id, model);
    } else {
        $.ajax({
            url: '/Admin/System',
            type: 'GET',
            data: { id: id },
            success: function (data) {
                var model = {
                    System: data,
                    Allegiances: selectLists.Allegiances,
                    Governments: selectLists.Governments,
                };

                RenderSystemEditor(id, model);
            }
        });
    }
}




function RenderSystemEditor(id, model) {
    var template = $.templates("#systemEditTemplate");
    var htmlOutput = template.render(model);

    $("#System").html(htmlOutput);
    $("#System #SystemId").val(id);
    $("#System #AllegianceId").val(model.System.AllegianceId);
    $("#System #GovernmentId").val(model.System.GovernmentId);
    $("#System #PermitRequired").val(model.System.PermitRequired);
    $("#SystemEditor #SystemName").on("change", CheckSystemNameAndGetLocation);

    $("#SystemEditor").validate({
        submitHandler: SubmitSystemChange,
        showErrors: function (errorMap, errorList) {
            $.each(this.successList, function (index, value) {
                return $(value).popover("hide");
            });
            return $.each(errorList, function (index, value) {
                var _popover;
                _popover = $(value.element).popover({
                    trigger: "manual",
                    placement: "bottom",
                    content: value.message,
                    template: "<div class=\"popover\"><div class=\"arrow\"></div><div class=\"popover-inner\"><div class=\"popover-content\"><p></p></div></div></div>"
                });
                // Bootstrap 3.x :      
                _popover.data("bs.popover").options.content = value.message;
                return $(value.element).popover("show");
            });
        }
    });
}




function HideSystemEditor(id) {
    if (id == 0) {
        $("#System").html('');
    } else {
        $.ajax({
            url: '/Admin/System',
            type: 'GET',
            data: { id: id },
            success: function (data) {
                var template = $.templates("#systemTemplate");

                var htmlOutput = template.render(data);

                $("#System").html(htmlOutput);
            }
        });
    }

}




function SubmitSystemChange(formData) {
    var form = $(formData);

    if (form.valid()) {
        var id = $("#System #SystemId").val();
        var data = {
            Id: $("#System #SystemId").val(),
            Name: $("#System #SystemName").val(),
            AllegianceId: $("#System #AllegianceId").val(),
            GovernmentId: $("#System #GovernmentId").val(),
            Economy: "",
            X: $("#System #X").val(),
            Y: $("#System #Y").val(),
            Z: $("#System #Z").val(),
            PermitRequired: $("#System #PermitRequired").is(':checked')
        };
        $.ajax({
            url: '/Admin/System',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (data) {
                RefreshSystemData();
                if (id == 0) { // new system
                    ShowDetails({ SystemId: data.Id });
                } else { // update
                    HideSystemEditor(data.Id);
                }
                if (data.RepResult != undefined) {
                    if (data.RepResult.RankUp) {
                        NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
                    }
                }
            }
        });
    }
}




function CheckSystemNameAndGetLocation() {
    var data = {
        data: {
            ver: 2,
            test: false,
            outputmode: 2,
            filter: {
                knownstatus: 1,
                systemname: $("#SystemEditor #SystemName").val(),
                cr: 1,
                date: "2014-09-18 12:34:56"
            }
        }
    };

    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: 'http://edstarcoordinator.com/api.asmx/GetSystems',
        data: JSON.stringify(data),
        dataType: 'json',
        success:
        function (result) {
            var data = result.d;
            if (data.status.input[0].status.msg = "Success") {
                var form = $("#SystemEditor");
                var name = $("#SystemName", form).val().toLowerCase();

                for (i = 0; i <= data.systems.length - 1; i++) {
                    if (data.systems[i].name.toLowerCase() == name) {
                        $("#SystemName", form).val(data.systems[i].name);
                        $("#X", form).val(data.systems[i].coord[0]);
                        $("#Y", form).val(data.systems[i].coord[1]);
                        $("#Z", form).val(data.systems[i].coord[2]);
                        $("#SaveSystemBtn", form).removeClass("hidden");
                        $("#System #SystemNotFoundMessage").addClass("hidden");
                        return true;
                    }
                }
                $("#System #SystemNotFoundMessage").removeClass("hidden");
                $("#SaveSystemBtn", form).addClass("hidden");
            }
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
};







/******************************************/
/*              Station                   */
/******************************************/
function ShowStationEditor(systemId, id) {
    if (id == 0) {
        var data = { Id: id, SystemId: systemId };
        var model = {
            Station: data,
            StationTypes: selectLists.StationTypes,
            StationsWithMarket: GetMarketStations(stationsData),
            Allegiances: selectLists.Allegiances,
            Governments: selectLists.Governments,
            Economies: selectLists.Economies,
        };

        RenderStationEditor(id, model, $("#NewStation"));

    } else {
        $.ajax({
            url: '/Admin/Station',
            type: 'GET',
            data: { id: id },
            success: function (data) {
                var model = {
                    Station: data,
                    StationTypes: selectLists.StationTypes,
                    Allegiances: selectLists.Allegiances,
                    Governments: selectLists.Governments,
                    Economies: selectLists.Economies,
                };

                RenderStationEditor(id, model, $("#Station" + id));
            }
        });
    }
}



function GetMarketStations(data) {
    var result = [];
    for (i = 0; i < data.length; i++) {
        if (data[i].HasMarket)
            result.push(data[i]);
    }
    return result;
}




function RenderStationEditor(id, model, target) {
    var template = $.templates("#stationEditorTemplate");
    var htmlOutput = template.render(model);
    $(target).html(htmlOutput);

    $("#StationTypeId", target).val(model.Station.StationTypeId);
    $("#Stations .fa-pencil").css("display", "none");
    $("#Stations #AddStationBtn").css("display", "none");
    $(".stationDisplay").css("opacity", "0.25");

    jQuery('.numbersOnly', target).numericInput({ allowFloat: true });

    if (id == 0) {
        $("#Station0Editor #HasMarket").on("change", function () {
            $("#CopyMarketFromStation").toggleClass("hidden");
        });
    } else {
        $("#AllegianceId", target).val(model.Station.AllegianceId);
        $("#GovernmentId", target).val(model.Station.GovernmentId);
        $("#EconomyId", target).val(model.Station.EconomyId);
        $("#SecondaryEconomyId", target).val(model.Station.SecondaryEconomyId);
    }

    $("#Station" + id + "Editor").validate({
        submitHandler: SubmitStationChange,
        rules: {
            DistanceFromJumpIn: {
                required: true,
                digits: true,
                min: 1
            }
        },
        showErrors: function (errorMap, errorList) {
            $.each(this.successList, function (index, value) {
                return $(value).popover("hide");
            });
            return $.each(errorList, function (index, value) {
                var _popover;
                _popover = $(value.element).popover({
                    trigger: "manual",
                    placement: "right",
                    content: value.message,
                    template: "<div class=\"popover\"><div class=\"arrow\"></div><div class=\"popover-inner\"><div class=\"popover-content\"><p></p></div></div></div>"
                });
                // Bootstrap 3.x :      
                _popover.data("bs.popover").options.content = value.message;
                return $(value.element).popover("show");
            });
        }
    });
}




function HideStationEditor(systemId, id) {
    if (id != 0) {
        $.ajax({
            url: '/Admin/Station',
            type: 'GET',
            data: { id: id },
            success: function (data) {
                $("#Stations .fa-pencil").css("display", "inline");
                $(".stationDisplay").css("opacity", "1");
                $("#Stations #AddStationBtn").css("display", "inline");
                var template = $.templates("#stationTemplate");

                var htmlOutput = template.render(data);

                $("#Station" + id).html(htmlOutput);

            }
        });
    } else {
        $("#NewStation").html("");
        $.ajax({
            url: '/Admin/SystemStations',
            type: 'GET',
            data: { id: systemId },
            success: function (data) {
                var template = $.templates("#systemStationsTemplate");
                var stationData = {
                    "CurrentMarketStationId": data.CurrentMarketStationId,
                    "stations": data.Stations,
                    "systemId": data.SystemId
                };
                var htmlOutput = template.render(stationData);

                $("#Stations").html(htmlOutput);
            }
        });
    }

}




function SubmitStationChange(formData) {
    var form = $(formData);
    var id = $("#StationId", form).val();
    var systemId = $("#SystemId", form).val();
    var copyMarketFromStationId = 0;
    if (id == 0 && $("#HasMarket", form).is(':checked')) {
        copyMarketFromStationId = $("#CopyMarketFromStationId", form).val();
    }
    if (form.valid()) {
        var data = {
            Id: id,
            SystemId: systemId,
            Name: $("#StationName", form).val(),
            FactionName: $("#FactionName", form).val(),
            StationTypeId: $("#StationTypeId", form).val(),
            HasBlackmarket: $("#HasBlackmarket", form).is(':checked'),
            HasMarket: $("#HasMarket", form).is(':checked'),
            HasOutfitting: $("#HasOutfitting", form).is(':checked'),
            HasShipyard: $("#HasShipyard", form).is(':checked'),
            HasRepairs: $("#HasRepairs", form).is(':checked'),
            HasRefuel: $("#HasRefuel", form).is(':checked'),
            HasRearm: $("#HasRearm", form).is(':checked'),
            AllegianceId: $("#AllegianceId", form).val(),
            GovernmentId: $("#GovernmentId", form).val(),
            EconomyId: $("#EconomyId", form).val(),
            SecondaryEconomyId: $("#SecondaryEconomyId", form).val(),
            DistanceFromJumpIn: $("#DistanceFromJumpIn", form).val(),
            CopyMarketFromStationId: copyMarketFromStationId
        };
        $.ajax({
            url: '/Admin/Station',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (data) {
                RenderStations(systemId);
                if (data.RepResult != undefined) {
                    if (data.RepResult.RankUp) {
                        NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
                    }
                }
            }
        });
    }
}




function DeleteStation(id, systemId) {
    var data = {
        Id: id
    }
    $.ajax({
        url: '/Admin/DeleteStation',
        type: 'POST',
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data) {
            RenderStations(systemId);
        }
    });
}







/******************************************/
/*            Commodities                 */
/******************************************/
function AddAllCommoditiesToCategory() {
    var data = {
        id: stationData.CurrentMarketStationId,
        categoryId: $("#CategoryId").val()
    }
    $.ajax({
        url: '/Admin/AddAllCommoditiesToCategory',
        type: 'POST',
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data) {
            RenderStationCommodityList(data);
        }
    });
}




function AddOneToCategory() {
    //Get the selected CategoryId 
    var categoryId = $("#CategoryId").val();

    // Find the commodities list for this category
    var commodities;
    for (var i = 0; i < selectLists.CommodityCategories.length;i++) {
        if (selectLists.CommodityCategories[i].Value == categoryId) {
            commodities = selectLists.CommodityCategories[i].Commodities;
            break;
        }
    }

    // Prep the model
    var data = {
        CategoryId: categoryId,
        Id : 0,
        Buy: 0,
        Sell: 0,
        Commodities: commodities
    }

    // Render it
    var template = $.templates("#stationCommodityAddTemplate");
    var htmlOutput = template.render(data);
    $('<tr id="StationCommodity0">' + htmlOutput + '</tr>').prependTo('#CommodityList');

    jQuery('#StationCommodity0 .numbersOnly').numericInput();
}




function StationCommodityButtonChange(id) {
    var targetRow = $("#CommodityList #StationCommodity" + id);
    $(".saveConfirm .btnLabel", targetRow).html(" Save");
    $(".saveConfirm .fa", targetRow).removeClass("fa-thumbs-up").addClass("fa-check");
    if (id != 0) {
        $(".saveConfirm", targetRow).prop("href", "javascript:SaveStationCommodity(" + id + ")");
    } else {
        $(".saveConfirm", targetRow).prop("href", "javascript:AddStationCommodity()");
    }

   
    if (id != 0) {
        $(".deleteCancel .btnLabel", targetRow).html(" Cancel");
        $(".deleteCancel .fa", targetRow).removeClass("fa-trash").addClass("fa-times");
        $(".deleteCancel", targetRow).prop("href", "javascript:CancelStationCommodityChange(" + id + ")");
    } 
    
}




function AddStationCommodity() {
    var targetRow = $("#StationCommodity0");
    var data = {
        Id: 0,
        StationId: stationData.CurrentMarketStationId,
        CommodityId: $("#CommodityIdSelector", targetRow).val(),
        Buy: $("#buy0", targetRow).val(),
        Sell: $("#sell0", targetRow).val(),
        CategoryId: $("#CategoryId").val()
    }

    $('#CommodityIdSelector', targetRow).popover('destroy');
    $('#sell0', targetRow).popover('destroy');
    $('#buy0', targetRow).popover('destroy');

    // do validation, popovers
    var valid = true;
    if (data.CommodityId == "") {
        valid = false;
        $('#CommodityIdSelector', targetRow).popover({ placement: "bottom" }).popover('show');
    }
    if (data.Sell < 1) {
        valid = false;
        $('#sell0', targetRow).popover({ placement: "bottom" }).popover('show');
    }
    
    if (valid) {
        $.ajax({
            url: '/Admin/AddEditStationCommodity',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (data) {
                RenderStationCommodityList(data);
                if (data.RepResult != undefined) {
                    if (data.RepResult.RankUp) {
                        NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
                    }
                }
            }, error: function (e, x, exception) {
                $("#StationCommodity0 .msg").html(exception);
            }
        });
    }
}




function CancelAddStationCommodity() {
    $("#StationCommodity0").remove();
}




function SaveStationCommodity(id) {
    var targetRow = $("#StationCommodity" + id);
    var data = {
        Id: id,
        StationId: stationData.CurrentMarketStationId,
        CommodityId: $("#CommodityId" + id, targetRow).val(),
        Buy: $("#buy" + id, targetRow).val(),
        Sell: $("#sell" + id, targetRow).val(),
        CategoryId: $("#CategoryId").val()
    }

    $('#sell' + id, targetRow).popover('destroy');
    $('#buy' + id, targetRow).popover('destroy');

    // do validation, popovers
    var valid = true;
    if (data.Sell < 1) {
        valid = false;
        $("#sell" + id, targetRow).popover({ placement: "bottom" }).popover('show');
    }
    if (valid) {
        $.ajax({
            url: '/Admin/AddEditStationCommodity',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (data) {
                if (data.RepResult != undefined) {
                    if (data.RepResult.RankUp) {
                        NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
                    }
                }
                RenderStationCommodityList(data);
            }
        });
    }
}




function CancelStationCommodityChange(id) {
    $("#CommodityList tr").css("opacity", "1");
    var targetRow = $("#StationCommodity" + id);
    $('#CommodityIdSelector', targetRow).popover('destroy');
    $('#sell0', targetRow).popover('destroy');
    $('#buy0', targetRow).popover('destroy');

    $.ajax({
        url: '/Admin/StationCommodity',
        type: 'GET',
        data: {id:id},
        contentType: "application/json",
        success: function (data) {

            $("#buy" + id, targetRow).val(data.Buy);
            $("#sell" + id, targetRow).val(data.Sell);

            $(".saveConfirm .btnLabel", targetRow).html(" Confirm");
            $(".saveConfirm .fa", targetRow).removeClass("fa-check").addClass("fa-thumbs-up");
            $(".saveConfirm", targetRow).prop("href", "javascript:ConfirmStationCommodity(" + id + ")");

            $(".deleteCancel .btnLabel", targetRow).html(" Delete");
            $(".deleteCancel .fa", targetRow).removeClass("fa-times").addClass("fa-trash");
            $(".deleteCancel", targetRow).prop("href", "javascript:DeleteStationCommodity(" + id + ")");
        }
    });
}




function ConfirmStationCommodity(id) {
    $.ajax({
        url: '/Admin/ConfirmStationCommodity',
        type: 'POST',
        data: JSON.stringify({ id: id, categoryId: $("#CategoryId").val() }),
        contentType: "application/json",
        success: function (data) {
            if (data.RepResult != undefined) {
                if (data.RepResult.RankUp) {
                    NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
                }
            }
            RenderStationCommodityList(data);
        }
    });
}




function ConfirmManyStationCommodities(id) {
    $.ajax({
        url: '/Admin/ConfirmManyStationCommodities',
        type: 'POST',
        data: JSON.stringify({ id: id, categoryId: $("#CategoryId").val() }),
        contentType: "application/json",
        success: function (data) {
            if (data.RepResult != undefined) {
                if (data.RepResult.RankUp) {
                    NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
                }
            }
            RenderStationCommodityList(data);
        }
    });
}




function DeleteStationCommodity(id) {
    $.ajax({
        url: '/Admin/DeleteStationCommodity',
        type: 'POST',
        data: JSON.stringify({ id: id, categoryId: $("#CategoryId").val() }),
        contentType: "application/json",
        success: function (data) {
            RenderStationCommodityList(data);
        }
    });
}








// jQuery plugin: PutCursorAtEnd 1.0
// http://plugins.jquery.com/project/PutCursorAtEnd
// by teedyay
//
// Puts the cursor at the end of a textbox/ textarea

// codesnippet: 691e18b1-f4f9-41b4-8fe8-bc8ee51b48d4
(function ($) {
    jQuery.fn.putCursorAtEnd = function () {
        return this.each(function () {
            $(this).focus()

            // If this function exists...
            if (this.setSelectionRange) {
                // ... then use it
                // (Doesn't work in IE)

                // Double the length because Opera is inconsistent about whether a carriage return is one character or two. Sigh.
                var len = $(this).val().length * 2;
                this.setSelectionRange(len, len);
            }
            else {
                // ... otherwise replace the contents with itself
                // (Doesn't work in Google Chrome)
                $(this).val($(this).val());
            }

            // Scroll to the bottom, in case we're in a tall textarea
            // (Necessary for Firefox and Google Chrome)
            this.scrollTop = 999999;
        });
    };
})(jQuery);