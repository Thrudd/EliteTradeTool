/******************************************/
/*               Utilitys                 */
/******************************************/
function supports_html5_storage() {
    try {
        return 'localStorage' in window && window['localStorage'] !== null;
    } catch (e) {
        return false;
    }
}

$.tablesorter.addParser({
    // set a unique id 
    id: 'thousands',
    is: function (s) {
        // return false so this parser is not auto detected 
        return false;
    },
    format: function (s) {
        // format your data for normalization 
        return s.replace(/,/g, '');
    },
    // set type, either numeric or text 
    type: 'numeric'
});

$.tablesorter.addParser({
    // set a unique id 
    id: 'distance',
    is: function (s) {
        // return false so this parser is not auto detected 
        return false;
    },
    format: function (s) {
        // format your data for normalization 
        return s.replace('ly', '');
    },
    // set type, either numeric or text 
    type: 'numeric'
});

var utility = {
    escapeQuotes: function (string) {
        return string.replace(/'/g, "\\'");
    },
    unescapeQuotes: function (string) {
        return string.replace(/\\'/g, "'");
    }
};

function maxLengthCheck(object) {
    if (object.value.length > object.maxLength)
        object.value = object.value.slice(0, object.maxLength)
}






/******************************************/
/*        Interface Functions             */
/******************************************/
function ShowMainPanel(name, title, page) {
    //GetUserLocationAndShipDetails();
    $(currentMainPanel + "Btn").removeClass("active");
    $(currentMainPanel).addClass("hidden");
    $(name + "Btn").addClass("active");
    $(name).removeClass("hidden");
    currentMainPanel = name;

    ga('set', {
        page: page,
        title: title
    });
    ga('send', 'pageview');

    switch (name) {
        case "#TradeCalculator":
            $("#CalcTable").floatThead("reflow");
            break;
        case "#Search":
            $("#SearchResultTable").floatThead("reflow");
            break;
    }
}

function ShowCalcSpinner() {
    $("#CalcSpinner").removeClass("hidden");
}

function ShowDataSpinner() {
    $("#DataSpinner").removeClass("hidden");
}

function ShowSearchSpinner() {
    $("#SearchSpinner").removeClass("hidden");
}

function ShowRouteSpinner() {
    $("#RouteSpinner").removeClass("hidden");
}

function ShowFindTradesSpinner() {
    $("#FindTradesSpinner").removeClass("hidden");
}

function ShowMissingStationsSpinner() {
    $("#MissingStationSpinner").removeClass("hidden");
}

function ShowDataListing() {
    $("#DataListing").removeClass("hidden");
    $("#StationData").addClass("hidden");
    $("#DataSpinner").addClass("hidden");
}

function ShowStationData() {
    switch (currentMainPanel) {
        case "#TradeCalculator":
            $("#CalcListing").addClass("hidden");
            $("#CalcStationData").removeClass("hidden");
            $("#CalcSpinner").addClass("hidden");
            $("#CalcTable").floatThead('destroy');
            break;
        case "#Search":
            $("#SearchListing").addClass("hidden");
            $("#SearchStationData").removeClass("hidden");
            $("#SearchSpinner").addClass("hidden");
            $("#SearchResultTable").floatThead('destroy');
            break;
        case "#FindTrades":
            $("#FindTradesListing").addClass("hidden");
            $("#FindTradesStationData").removeClass("hidden");
            $("#FindTradesSpinner").addClass("hidden");
            $("#FindTradesResultTable").floatThead('destroy');
            break;
    }

};

function ShowResults() {
    switch (currentMainPanel) {
        case "#TradeCalculator":
            $("#CalcListing").removeClass("hidden");
            $("#CalcStationData").addClass("hidden");
            $("#CalcSpinner").addClass("hidden");
            $("#CalcTable").floatThead({ scrollingTop: 50 });
            break;
        case "#Search":
            $("#SearchListing").removeClass("hidden");
            $("#SearchStationData").addClass("hidden");
            $("#SearchSpinner").addClass("hidden");
            $("#SearchResultTable").floatThead({ scrollingTop: 50 });
            break;
        case "#FindTrades":
            $("#FindTradesListing").removeClass("hidden");
            $("#FindTradesStationData").addClass("hidden");
            $("#FindTradesSpinner").addClass("hidden");
            $("#FindTradesResultTable").floatThead({ scrollingTop: 50 });
            break;
    }
};

function ShowRouteListing() {
    $("#RouteListing").removeClass("hidden");
    $("#RouteSpinner").addClass("hidden");
}

function SwapLocations() {
    var currentStartStation = $("#StartStationId").val();
    var currentEndStation = $("#EndStationId").val();
    $("#StartStationId").val(currentEndStation);
    $("#EndStationId").val(currentStartStation);
}





var currentSize;
function CheckSize() {
    var xs = window.matchMedia("(min-width: 480px) and (max-width: 767px)");
    var sm = window.matchMedia("(min-width: 768px) and (max-width: 991px)");
    var md = window.matchMedia("(min-width: 992px) and (max-width: 1199px)");
    var lg = window.matchMedia("(min-width: 1200px)");
    if (xs.matches && currentSize != "xs") {
        currentSize = "xs";
        XtraSmallSize();
    }
    if (sm.matches && currentSize != "sm") {
        currentSize = "sm";
        SmallSize();
    }
    if (md.matches && currentSize != "md") {
        currentSize = "md";
        LargeSize();
    }
    if (lg.matches && currentSize != "lg") {
        currentSize = "lg";
        LargeSize();
    }
}

function XtraSmallSize() {
    // Clean up left over styles from justifyNav()
    $('#Donate').appendTo('#DonateSmall');
}

function SmallSize() {
    // Clean up left over styles from justifyNav()
    $('#Donate').appendTo('#DonateLarge');
}

function LargeSize() {
    // Clean up left over styles from justifyNav()
    $('#Donate').appendTo('#DonateLarge');
}

function DoTopNav() {
    $('#Donate').appendTo('#DonateLarge');
}



/******************************************/
/*       User Data Persistence            */
/******************************************/
var userData = {};
var currentVersion = 1.3
function LinkUserEntryFormElements() {
    $(".cash").link(true, userData);
    $(".cargo").link(true, userData);
    $(".minProfit").link(true, userData);
    $(".MaxDistanceFromJumpIn").link(true, userData);
    $("#MaxDistanceBetweenSystems").link(true, userData);
    $("#StartSystem").link(true, userData);
    $("#EndSystem").link(true, userData);
    $("#ExcludeOutposts").link(true, userData);
    $("#ExcludePlanets").link(true, userData);
}

function UpdateUserLocationAndShipDetails() {
    if (supports_html5_storage()) {
        localStorage["userData"] = JSON.stringify(userData);
    }
}

function GetUserLocationAndShipDetails() {
    if (supports_html5_storage()) {
        ud = localStorage["userData"];
        if (typeof ud != 'undefined') {
            userData = JSON.parse(ud);
            if (typeof userData.Ver == 'undefined' || userData.Ver != currentVersion) {
                localStorage.clear();
                userData = {
                    Ver: currentVersion,
                    StartSystem: "",
                    StartStationId: null,
                    EndSystem: "",
                    EndStationId: null,
                    Cash: 1000,
                    Cargo: 4,
                    MaxDistanceFromJumpIn: 1000,
                    MaxDistanceBetweenSystems: 50,
                    MinProfit: 500,
                    ExcludeOutposts: 0,
                    ExcludePlanets: 0
                };
                localStorage["userData"] = JSON.stringify(userData);
            }

            $("#TradeCalcForm #StartSystem").val(userData.StartSystem);
            $("#TradeCalcForm #EndSystem").typeahead('val', userData.EndSystem);

            $("#SearchForm #CurrentLocation").typeahead('val', userData.StartSystem);

            return true;
        }
    }
    userData = {
        Ver: currentVersion,
        StartSystem: "",
        StartStationId: null,
        EndSystem: "",
        EndStationId: null,
        Cash: 1000,
        Cargo: 4,
        MaxDistanceFromJumpIn: 1000,
        MaxDistanceBetweenSystems: 50,
        MinProfit: 500,
        ExcludeOutposts: 0,
        ExcludePlanets: 0
    };
}








/******************************************/
/*          Trade Calculation             */
/******************************************/
function SetUpCalculator() {
    jQuery('.numbersOnly').numericInput();
    jQuery('.floatOnly').numericInput({ allowFloat: true });

    var routeTemplate = $.templates("#RouteTemplate");
    routeTemplate.link("#RoutePlanner", routePlan);

    $("#CalculateBtn").on("click", NewCalculation);

    if (userData.StartSystem != "") {
        var gotStations = GetStationsInSystem("StartSystem", userData.StartSystem, true);
    }
    if (userData.EndSystem != "") {
        GetStationsInSystem("EndSystem", userData.EndSystem, true);
    }
}

function NewCalculation() {
    var validator = $("#TradeCalcForm").validate();
    if (!validator.form())
        return false;
    
    var startStationId = $("#TradeCalcForm #StartSystemStationId").val();
    var endStationId = $("#TradeCalcForm #EndSystemStationId").val();

    userData.StartStationId = startStationId != "" ? startStationId : null;
    userData.EndStationId = endStationId != "" ? endStationId : null;

    //Update the user settings
    UpdateUserLocationAndShipDetails();

    var querydata = {
        StartSystem: userData.StartSystem,
        StartStationId: userData.StartStationId,
        EndSystem: userData.EndSystem,
        EndStationId: userData.EndStationId,
        Cargo: userData.Cargo,
        Cash: userData.Cash,
        MinProfit: userData.MinProfit,
        MaxDistanceFromJumpIn: userData.MaxDistanceFromJumpIn,
        SearchRange: $("#TradeCalcForm #SearchRange:checked").val(),
        ExcludeOutposts: $("#TradeCalcForm #ExcludeOutposts:checked").val(),
        ExcludePlanets: $("#TradeCalcForm #ExcludePlanets:checked").val()
    };


    ShowCalcSpinner();

    $.ajax({
        url: 'api/EliteTradingTool/Calculator',
        type: 'POST',
        data: JSON.stringify(querydata),
        contentType: "application/json",
        success: function (data) {
            // Show/hide stuff
            $("#RoutePlanner").addClass("hidden");
            $("#CalcListing").removeClass("hidden");

            // Reset route planner readouts
            $.observable(routePlan).setProperty("TotalProfit", 0);
            $.observable(routePlan).setProperty("TotalProfitFormatted", accounting.formatNumber(0));
            $.observable(routePlan.Routes).remove(0, routePlan.Routes.length);

            // Update the calculator results
            UpdateCalculatorResults(data);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
    return false;
}

function UpdateCalculatorResults(data) {
    $("#CalcSpinner").addClass("hidden");

    // Check if there were any results
    if (data.Results.length > 0) {

        // Set the template data
        var calcListing = {
            Cash: data.Cash,
            Cargo: data.Cargo,
            Location: data.Location,
            JumpRange: userData.JumpRange,
            Results: []
        };

        $.each(data.Results, function (key, item) {

            var calcResultRow = {
                CommodityName: item.CommodityName,
                Buy: accounting.formatNumber(item.Buy),
                Supply: item.Supply,
                SupplyAmount: item.SupplyAmount,
                BuyUnformatted: item.Buy,
                Demand: item.Demand,
                DemandAmount: item.DemandAmount,
                Sell: accounting.formatNumber(item.Sell),
                SellUnformatted: item.Sell,
                GalacticAveragePrice: item.GalacticAveragePrice,
                BuyUpdatedBy: item.BuyUpdatedBy,
                BuyLastUpdate: item.BuyLastUpdate,
                SellUpdatedBy: item.SellUpdatedBy,
                SellLastUpdate: item.SellLastUpdate,
                Profit: accounting.formatNumber(item.Profit),
                ProfitUnformatted: item.Profit,
                Source: item.Source,
                SourceStationId: item.SourceStationId,
                SourceStationTypeIcon: item.SourceStationTypeIcon,
                SourceStationTypeName: item.SourceStationTypeName,
                SourceStationAllegiance: item.SourceStationAllegiance,
                SourceSystemId: item.SourceSystemId,
                SourceStationDistance: item.SourceStationDistance,
                Destination: item.Destination,
                DestinationStationId: item.DestinationStationId,
                DestinationStationTypeIcon: item.DestinationStationTypeIcon,
                DestinationStationTypeName: item.DestinationStationTypeName,
                DestinationStationAllegiance: item.DestinationStationAllegiance,
                DestinationSystemId: item.DestinationSystemId,
                DestinationStationDistance: item.DestinationStationDistance,
                Distance: item.Distance,
                Qty: item.Qty,
                TotalProfit: accounting.formatNumber(item.TotalProfit),
                TotalProfitUnformatted: item.TotalProfit,
                SourceEncoded: utility.escapeQuotes(item.Source),
                DestinationEncoded: utility.escapeQuotes(item.Destination),
                SourceSystemNameEncoded: utility.escapeQuotes(item.SourceSystemName),
                DestinationSystemNameEncoded: utility.escapeQuotes(item.DestinationSystemName),
                SourcePermitRequired: item.SourcePermitRequired,
                DestinationPermitRequired: item.DestinationPermitRequired
            };

            // Add the results to the template data
            calcListing.Results.push(calcResultRow);
        });


        // Show the instructions panel
        $("#Instructions").removeClass("hidden");

        // Configure and render the template
        var template = $.templates("#CalcListingTemplate");
        var htmlOutput = template.render(calcListing);
        $("#CalcListing").html(htmlOutput);

        // Apply tablesorter
        $("#CalcTable").tablesorter({
            headers: {
                1: {
                    sorter: 'distance' 
                },
                3: {//zero-based column index
                    sorter: 'thousands'
                },
                4: {//zero-based column index
                    sorter: 'thousands'
                },
                5: {//zero-based column index
                    sorter: 'thousands'
                },
                6: {//zero-based column index
                    sorter: 'thousands'
                },
                8: {//zero-based column index
                    sorter: 'thousands'
                }
            }
        });
        RemoveRoutesFromTableOnSort("#CalcTable");
        //$("#CalcTable").floatThead({scrollingTop:50,useAbsolutePositioning:false});
    } else {
        $("#Instructions").addClass("hidden");
        $("#CalcListing").html("<div style='clear:both;padding-top:10px;'><hr /><p>No results returned.</p></div>");
    }
    
    if (data.RepResult != undefined) {
        if (data.RepResult.RankUp) {
            NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
        }
    }
}

function ReverseCalculatorQuery() {
    var origStartSystem, origEndSystem, origStartStationId, origEndStationId, origStartStations, origEndStations;
    origStartSystem = $("#TradeCalcForm #StartSystem").val();
    origEndSystem = $("#TradeCalcForm #EndSystem").val();
    origStartStationId = $("#TradeCalcForm #StartSystemStationId").val();
    origEndStationId = $("#TradeCalcForm #EndSystemStationId").val();
    origStartStations = $("#StartSystemStations option").detach();
    origEndStations = $("#EndSystemStations option").detach();
    $("#TradeCalcForm #StartSystem").val(origEndSystem);
    $("#TradeCalcForm #EndSystem").val(origStartSystem);

    $("#StartSystemStations select").append(origEndStations);
    $("#EndSystemStations select").append(origStartStations);
    $("#TradeCalcForm #StartSystemStations select").val(origEndStationId);
    $("#TradeCalcForm #EndSystemStations select").val(origStartStationId);
    userData.StartSystem = origEndSystem;
    userData.EndSystem = origStartSystem;
    userData.StartStationId = origEndStationId;
    userData.EndStationId = origStartStationId;
}

function GetStationsInSystem(container, systemName, setSelected) {
    var target = $("#" + container + "StationsGroup");

    var gotStations  = $.ajax({
        url: '/api/EliteTradingTool/StationsInSystem',
        type: 'Get',
        data: { systemName: systemName },
        contentType: "application/json",
        success: function (data) {
            target.removeClass("hidden");
            if (data.Success) {
                $("#" + container + "Stations").html('<select class="form-control" id="' + container + 'StationId"></select>');
                if (data.length > 1) {
                    $("#"+container + "StationId", target).find('option').remove().end().append('<option value="">Any</option>');
                }
                $.each(data.Stations, function (i, obj) {
                    var option
                    if (obj.StationId == 0) {
                        option = "<option value=''>" + obj.Station + "</option>";
                    } else {
                        option = "<option value=" + obj.StationId + ">" + obj.Station + "</option>";
                    }

                    $(option).appendTo("#" + container + "StationId", target);
                });
                CheckCalcSubmit();
            } else {
                $("#" + container + "Stations").html("<p class='form-control-static'>" + data.Message + "</p>");
                HideCalcSubmit();
            }

        }, error: function (e, x, exception) {
            target.removeClass("hidden");
            $("#" + container + "Stations").html("<p class='form-control-static'>" + e.responseJSON.Message + "</p>");
            HideCalcSubmit();
        }
    });


    if (setSelected) {
        if (container == "StartSystem") {
            if (userData.StartStationId != null) {
                $.when(gotStations).then(function () {
                    $("#StartSystemStationId").val(userData.StartStationId);
                });
            }
        } else {
            if (userData.EndStationId != null) {
                $.when(gotStations).then(function () {
                    $("#EndSystemStationId").val(userData.EndStationId);
                });
            }
        }

    }
}

function CheckCalcSubmit() {
    var startSystem = $("#TradeCalcForm #StartSystem").val();
    var endSystem = $("#TradeCalcForm #EndSystem").val();
    if (startSystem != "" && endSystem != "") {
        $("#CalcBtns").removeClass("hidden");
        $("#CalculateBtn").removeClass("hidden");
        $("#ReverseBtn").removeClass("hidden");
    } else if (startSystem == "" && endSystem == "") {
        HideCalcSubmit();
    } else {
        $("#CalcBtns").removeClass("hidden");
        $("#CalculateBtn").removeClass("hidden");
        $("#ReverseBtn").addClass("hidden");
    }
}

function HideCalcSubmit() {
    $("#CalcBtns").addClass("hidden");
    $("#ReverseBtn").addClass("hidden");
}

function ClearTypeAhead(name) {
    $("#" + name).typeahead('val', '');
    $("#" + name + "StationsGroup").addClass("hidden");
    if (name == "StartSystem") {
        userData.StartSystem = "";
        userData.StartStationId = null;
    } else {
        userData.EndSystem = "";
        userData.EndStationId = null;
    }
    $("#" + name + "Stations").html('');
    CheckCalcSubmit();
    UpdateUserLocationAndShipDetails();
}



/******************************************/
/*          Find Trades Form              */
/******************************************/
function SetUpFindTrades() {
    jQuery('.numbersOnly').numericInput();
    jQuery('.floatOnly').numericInput({ allowFloat: true });

    $("#FindTradesForm").validate({
        rules: {
            MaxDistanceFromJumpIn: {
                required: true,
                max: 2000
            }
        },
        submitHandler: FindTrades,
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

//function RangeMinCheck() {
//    var range = $("input[name=SearchRange]:checked", "#FindTradesForm").val();
//    switch (range) {
//        case "0":
//            bindMinRule(200);
//            break;
//        case "20":
//            bindMinRule(400);
//            break;
//        case "30":
//            bindMinRule(800);
//            break;
//        case "40":
//            bindMinRule(1000);
//            break;
//        case "60":
//            bindMinRule(1200);
//            break;
//    }
//}

//function bindMinRule(min) {
//    var settings = $('#FindTradesForm').validate().settings;

//    delete settings.rules.MinProfitPerTon;

//    settings.rules.MinProfitPerTon = {
//        required: true,
//        min: min
//    }
//};

function FindTrades() {
    //Update the user settings
    UpdateUserLocationAndShipDetails();

    var querydata = {
        SearchRange: $("#FindTradesForm #SearchRange:checked").val(),
        SystemName: $("#FindTradesForm #CurrentLocation").typeahead('val'),
        //MinProfitPerTon: $("#FindTradesForm #MinProfitPerTon").val(),
        ExcludeOutposts: $("#FindTradesForm #ExcludeOutposts:checked").val(),
        ExcludePlanets: $("#FindTradesForm #ExcludePlanets:checked").val(),
        //BiDirectional: $("#FindTradesForm #Bidirectional:checked").val(),
        MaxDistanceFromJumpIn: $("#FindTradesForm #MaxDistanceFromJumpIn").val(),
        MaxDistanceBetweenSystems: $("#FindTradesForm #MaxDistanceBetweenSystems").val()
    };

    ShowFindTradesSpinner();

    $.ajax({
        url: 'api/EliteTradingTool/FindTrades',
        type: 'POST',
        data: JSON.stringify(querydata),
        contentType: "application/json",
        success: function (data) {
            // Show/hide stuff
            $("#FindTradesListing").removeClass("hidden");

            // Display the search results
            FindTradesResults(data);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });

}

function FindTradesResults(data) {
    $("#FindTradesForm #FindTradesSpinner").addClass("hidden");
    //var Bidirectional = $("#FindTradesForm #Bidirectional:checked").val();
    // Check if there were any results
    if (data != null) {
        if (data.List.length > 0) {
            var dataProcessed = [];
            for (var i = 0; i < data.List.length; i++) {
                var currentItem = data.List[i];
                var item = {
                    OutgoingCommodityName: currentItem.OutgoingCommodityName,
                    ReturningCommodityName: currentItem.ReturningCommodityName,
                    OutgoingBuy: currentItem.OutgoingBuy,
                    OutgoingSell: currentItem.OutgoingSell,
                    OutgoingBuyLastUpdate: currentItem.OutgoingBuyLastUpdate,
                    OutgoingSellLastUpdate: currentItem.OutgoingSellLastUpdate,
                    ReturningBuy: currentItem.ReturningBuy,
                    ReturningSell: currentItem.ReturningSell,
                    ReturningBuyLastUpdate: currentItem.ReturningBuyLastUpdate,
                    ReturningSellLastUpdate: currentItem.ReturningSellLastUpdate,
                    OutgoingProfit: currentItem.OutgoingProfit,
                    ReturningProfit: currentItem.ReturningProfit,
                    TotalProfit: currentItem.TotalProfit,
                    Source: currentItem.Source,
                    SourceStationId: currentItem.SourceStationId,
                    SourceStationDistance: currentItem.SourceStationDistance,
                    SourceSystemId: currentItem.SourceSystemId,
                    Destination: currentItem.Destination,
                    DestinationStationId: currentItem.DestinationStationId,
                    DestinationSystemId: currentItem.DestinationSystemId,
                    DestinationStationDistance: currentItem.DestinationStationDistance,
                    Distance: currentItem.Distance
                }
                dataProcessed.push(item);
            }

            //if (Bidirectional == "True") {
                
            //} else {
            //    for (var i = 0; i < data.UniList.length; i++) {
            //        var currentItem = data.UniList[i];
            //        var item = {
            //            OutgoingCommodityName: currentItem.CommodityName,
            //            OutgoingBuy: currentItem.Buy,
            //            OutgoingSell: currentItem.Sell,
            //            OutgoingBuyLastUpdate: currentItem.BuyLastUpdate,
            //            OutgoingSellLastUpdate: currentItem.SellLastUpdate,
            //            OutgoingProfit: currentItem.Profit,
            //            TotalProfit: currentItem.Profit,
            //            Source: currentItem.Source,
            //            SourceStationId: currentItem.SourceStationId,
            //            SourceStationDistance: currentItem.SourceStationDistance,
            //            SourceSystemId: currentItem.SourceSystemId,
            //            Destination: currentItem.Destination,
            //            DestinationStationId: currentItem.DestinationStationId,
            //            DestinationSystemId: currentItem.DestinationSystemId,
            //            DestinationStationDistance: currentItem.DestinationStationDistance,
            //            Distance: currentItem.Distance
            //        }
            //        dataProcessed.push(item);
            //    }
            //}
            

            // Set the template data
            var FindTradesListing = {
                Results: dataProcessed
            };


            // Configure and render the template
            var template = $.templates("#FindTradesListingTemplate");
            var htmlOutput = template.render(FindTradesListing);
            $("#FindTradesListing").html(htmlOutput);

            
            $("#FindTradesTable").tablesorter({
                headers: {
                    1: {//zero-based column index
                        sorter: 'distance'
                    },
                    2: {
                        sorter: 'thousands'
                    }
                }
            });
            $("#FindTradesTable").floatThead({ scrollingTop: 50, useAbsolutePositioning: false });
        } else {
            $("#FindTradesListing").html("<hr style='margin-top:10px' /><p>No results returned.</p>");
        }
    }
}


function FindTradeReturns(data, currentItem, sourceStationId,destinationStationId, dataWithReturns){
    for (var i = 0; i < data.List.length; i++) {
        if (data.List[i].SourceStationId != sourceStationId && data.List[i].SourceStationId == destinationStationId && data.List[i].DestinationStationId == sourceStationId) {
            var item = {
                OutboundCommodityName: currentItem.CommodityName,
                ReturnCommodityName: data.List[i].CommodityName,
                OutboundBuy: currentItem.Buy, 
                OutboundSell: currentItem.Sell, 
                OutboundBuyLastUpdate: currentItem.BuyLastUpdate, 
                OutboundSellLastUpdate: currentItem.SellLastUpdate,
                ReturnBuy: data.List[i].Buy, 
                ReturnSell: data.List[i].Sell, 
                ReturnBuyLastUpdate: data.List[i].BuyLastUpdate, 
                ReturnSellLastUpdate: data.List[i].SellLastUpdate, 
                OutboundProfit: currentItem.Profit, 
                ReturnProfit: data.List[i].Profit,
                TotalProfit: currentItem.Profit + data.List[i].Profit,
                Source: currentItem.Source,
                SourceStationId: currentItem.SourceStationId,
                SourceStationDistance: currentItem.SourceStationDistance,
                SourceSystemId: currentItem.SourceSystemId,
                Destination: currentItem.Destination,
                DestinationStationId: currentItem.DestinationStationId,
                DestinationSystemId: currentItem.DestinationSystemId,
                DestinationStationDistance: currentItem.DestinationStationDistance,
                Distance: currentItem.Distance
            }

            dataWithReturns.push(item);
        }
    }
}


function FindHighestTradeCommodity(row, sourceStationId, destinationStationId) {
    $.ajax({
        url: 'api/EliteTradingTool/FindHighestTradeCommodity',
        type: 'POST',
        data: JSON.stringify({SourceStationId:sourceStationId, DestinationStationId:destinationStationId}),
        contentType: "application/json",
        success: function (data) {
            if (data.Profit > 0) {
                $(".returnLink", row).addClass("hidden");
                var template = $.templates("#FindTradesReturnTemplate");
                var htmlOutput = template.render(data);

                $(".outBound", row).after(htmlOutput);
                var profitTag = $(".profit", row);
                var profit = parseInt(profitTag.html()) + data.Profit;
                profitTag.html(profit);
                $("#FindTradesTable").trigger("update");
            } else {
                $(".returnLink", row).addClass("hidden");
                $(".outBound", row).after("<p>No profitable return trade.</p>");
            }
            
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}






/******************************************/
/*             Search Form                */
/******************************************/
function NewSearch() {
    var validator = $("#SearchForm").validate();
    if (!validator.form())
        return false;

    //Update the user settings
    UpdateUserLocationAndShipDetails();

    var querydata = {
        Commodity: $("#SearchForm #Commodity").prop('checked'),
        Blackmarket: $("#SearchForm #Blackmarket").prop('checked'),
        Outfitting: $("#SearchForm #Outfitting").prop('checked'),
        Shipyard: $("#SearchForm #Shipyard").prop('checked'),
        Repairs: $("#SearchForm #Repairs").prop('checked'),
        Refuel: $("#SearchForm #Refuel").prop('checked'),
        Rearm: $("#SearchForm #Rearm").prop('checked'),
        Allegiance: $("#SearchForm #Allegiance").prop('checked'),
        Government: $("#SearchForm #Government").prop('checked'),
        Economy: $("#SearchForm #Economy").prop('checked'),
        SearchType: $("#SearchForm input[name=SearchType]:checked").val(),
        AllegianceId: $("#SearchForm #AllegianceId").val(),
        GovernmentId: $("#SearchForm #GovernmentId").val(),
        CommodityId: $("#SearchForm #CommodityId").val(),
        EconomyId: $("#SearchForm #EconomyId").val(),
        SearchRange: $("#SearchForm #SearchRange:checked").val(),
        CurrentLocation: $("#SearchForm #CurrentLocation").typeahead('val'),
        FactionName: $("#SearchForm #FactionName").val(),
        ExcludeOutposts: $("#SearchForm #ExcludeOutposts:checked").val(),
        ExcludePlanets: $("#SearchForm #ExcludePlanets:checked").val()
    };
    //maxJumps = $("#SearchForm #MaxJumps").val();
    ShowSearchSpinner();

    $.ajax({
        url: 'api/EliteTradingTool/Search',
        type: 'POST',
        data: JSON.stringify(querydata),
        contentType: "application/json",
        success: function (data) {
            // Show/hide stuff
            $("#SearchListing").removeClass("hidden");
            $("#StationData").addClass("hidden");

            // Display the search results
            SearchResults(data);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
    return false;
}

function SearchResults(data) {
    $("#SearchForm #SearchSpinner").addClass("hidden");

    // Check if there were any results
    if (data.Results != null) {
        if (data.Results.length > 0) {
            // Set the template data
            var searchListing = {
                //StartingSystem: data.Query.CurrentLocation,
                Query: data.Query,
                JumpRange: 0,
                Results: []
            };


            // On success, 'data' contains a list of products.
            //$.each(data.Results, function (key, item) {
            //    // Get the route


            //    var searchResultRow = {
            //        System: item.System,
            //        Buy: item.Buy,
            //        Sell: item.Sell,
            //        LastUpdate: item.LastUpdate,
            //        Station: item.Station,
            //        EconomyString: item.Station.EconomyString,
            //        Distance: item.Distance == 0 ? 0 : item.Distance
            //    };

            //    // Add the results to the template data
            //    searchListing.Results.push(searchResultRow);

            //});


            // Configure and render the template
            var template = $.templates("#SearchTemplate");
            var htmlOutput = template.render(data);
            $("#SearchListing").html(htmlOutput);


            // Apply tablesorter
            if (data.Query.Commodity) {
                $("#SearchResultTable").tablesorter({
                    headers: {
                        2: {//zero-based column index
                            sorter: 'thousands'
                        },
                        3: {
                            sorter: 'distance'
                        }
                    }
                });
            } else {
                $("#SearchResultTable").tablesorter({
                    headers: {
                        2: {
                            sorter: 'distance'
                        }
                    }
                });
            }

           // $("#SearchResultTable").floatThead({ scrollingTop: 50, useAbsolutePositioning: false });
        } else {
            $("#SearchListing").html("<hr /><p>No results returned.</p>");
        }
        
    } else {
        $("#SearchListing").html("<hr /><p>No results returned. Starting system may not have a station assigned.</p>");
    }

    if (data.RepResult != undefined) {
        if (data.RepResult.RankUp) {
            NewRankAchieved(data.RepResult.Title, data.RepResult.Badge);
        }
    }
}

function RemoveRoutesFromTableOnSort(container) {
    $(container).bind("sortStart", function () {
        $(container + " .routeRow").parent().remove();
        $(container + " .dummyRow").parent().remove();
        $(container + " .getRoute").css("display", "inline");
    });
}







/******************************************/
/*          Trade Route Planner           */
/******************************************/
var routePlan = {
    TotalProfit: 0,
    TotalProfitFormatted: 0,
    Routes: []
};

function Route(id, source, destination, sourceStationId, sourceSystemName, destinationStationId, destinationSystemName, distance, commodity, buy, sell, qty, profit, totalprofit) {
    this.Id = id;
    this.Source = source;
    this.SourceStationId = sourceStationId;
    this.SourceSystemName = sourceSystemName;
    this.Destination = destination;
    this.DestinationStationId = destinationStationId;
    this.DestinationSystemName = destinationSystemName;
    this.Distance = distance;
    this.Commodity = commodity;
    this.Buy = buy;
    this.Sell = sell;
    this.Qty = qty;
    this.Profit = profit;
    this.ProfitFormatted = accounting.formatNumber(profit);
    this.RunProfit = totalprofit;
    this.RunProfitFormatted = accounting.formatNumber(totalprofit);
}

var routeId = 0;
function AddToRoutePlan(source, destination, sourceStationId, sourceSystemName, destinationStationId, destinationSystemName, distance, commodity, buy, sell, qty, profit, totalprofit) {
    $("#RoutePlanner").removeClass("hidden");
    $.observable(routePlan).setProperty("TotalProfit", routePlan.TotalProfit + totalprofit);
    $.observable(routePlan).setProperty("TotalProfitFormatted", accounting.formatNumber(routePlan.TotalProfit));
    $.observable(routePlan.Routes).insert(new Route(routeId, utility.unescapeQuotes(source), utility.unescapeQuotes(destination), sourceStationId, sourceSystemName, destinationStationId, destinationSystemName, distance, commodity, buy, sell, qty, profit, totalprofit));
    routeId += 1;

    // work out what profit to add to the next step
    var profitCalc = 0;
    for (var i = 0, len = routePlan.Routes.length; i < len; i++) {
        profitCalc += routePlan.Routes[i].RunProfit;
    }
    $("#SwapBtn").removeClass("hidden");
    UpdateList(destinationStationId,destinationSystemName, profitCalc);
    $("#CalcTable").floatThead("reflow");
}

function RemoveFromRoutePlan(routeId) {
    var profitCalc = 0; // used to work out what to set the profit to once later steps have been removed

    // Loop through the collection
    for (var i = 0, len = routePlan.Routes.length; i < len; i++) {
        if (routePlan.Routes[i].Id == routeId) { // This is the entry to be removed

            $.observable(routePlan).setProperty("TotalProfit", profitCalc);
            $.observable(routePlan).setProperty("TotalProfitFormatted", accounting.formatNumber(profitCalc));

            // Check if we're just removing the first route
            if (i == 0) {
                $.observable(routePlan.Routes).remove(0, 1);
                if (routePlan.Routes.length == 0) {
                    NewCalculation();
                }
            } else {
                // remove the following routes and rerun the calculator
                UpdateList(routePlan.Routes[i].SourceStationId, routePlan.Routes[i].SourceSystemName, profitCalc);
                var numOfEntriesToRemove = routePlan.Routes.length - i;
                $.observable(routePlan.Routes).remove(i, numOfEntriesToRemove);
            }
            break;
        }
    }

    for (var i = 0, len = routePlan.Routes.length; i < len; i++) {
        profitCalc += routePlan.Routes[i].RunProfit;
    }
    $.observable(routePlan).setProperty("TotalProfit", profitCalc);
    $.observable(routePlan).setProperty("TotalProfitFormatted", accounting.formatNumber(profitCalc));
    if (routePlan.Routes.length == 0) {
        $("#RoutePlanner").addClass("hidden");
        $("#Instructions").addClass("hidden");
        $("#CalcListing").html('');
    }
    $("#CalcTable").floatThead("reflow");
}

function UpdateList(startStationId, startSystemName, profit) {
    var querydata = {
        StartSystem: startSystemName,
        StartStationId: startStationId,
        EndSystem: "",
        EndStationId: null,
        Cargo: userData.Cargo,
        Cash: parseInt(userData.Cash) + parseInt(profit),
        MinProfit: userData.MinProfit,
        SearchRange: $("#TradeCalcForm #SearchRange:checked").val(),
        ExcludeOutposts: $("#TradeCalcForm #ExcludeOutposts:checked").val(),
        ExcludePlanets: $("#TradeCalcForm #ExcludePlanets:checked").val(),
        MaxDistanceFromJumpIn: userData.MaxDistanceFromJumpIn
    };

    ShowCalcSpinner();

    $.ajax({
        url: 'api/EliteTradingTool/Calculator',
        type: 'POST',
        data: JSON.stringify(querydata),
        contentType: "application/json",
        success: UpdateCalculatorResults,
        error: function (data) {
            alert(data.responseText);
        }
    });
}





/******************************************/
/*               Rare Trades              */
/******************************************/
function SetUpRareTrades() {

    $("#RareTradesForm").validate({
        submitHandler: RareTrades,
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

function RareTrades() {
    $.ajax({
        url: '/api/EliteTradingTool/RareTrades',
        type: 'POST',
        data: JSON.stringify({ CurrentLocation: $("#RareTradesForm #CurrentLocation").val() }),
        contentType: "application/json",
        success: function (data) {
            var rareTradesListing = { RareTrades: data };
            // Configure and render the template
            var template = $.templates("#RareTradesTemplate");
            var htmlOutput = template.render(rareTradesListing);
            $("#RareTradesListing").html(htmlOutput);
            $("#RareTradesTable").tablesorter({
                headers: {
                    1: {//zero-based column index
                        sorter: 'thousands'
                    },
                    2: {
                        sorter: 'distance'
                    },
                    4: {
                        sorter: 'distance'
                    }
                }
            });
            $("#RareTradesTable").floatThead({ scrollingTop: 50, useAbsolutePositioning: false });
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
    return false;
}





/* ***************** */
/*     Data Lists    */
/* ***************** */
function SetupDataLists() {
    var form = $("#DataListsForm");
    $("input[type=radio][name=QueryType]", form).on("change", function () {
        var queryType = $("#QueryType:checked", form).val();
        $("#SystemGroup").removeClass("hidden");
        $("#SubmitGroup").removeClass("hidden");
        if (queryType == "System"){
            $("#CommodityGroup").addClass("hidden");
            $("#SystemLabel").text("System");
        } else {
            $("#CommodityGroup").removeClass("hidden");
            $("#SystemLabel").text("Current System");
        }
    });
    
    $(form).validate({
        submitHandler: DataLists,
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

function DataLists() {
    ShowDataSpinner();
    var form = $("#DataListsForm");
    var query = {
        SystemName:  $("#SystemName", form).val(),
        QueryType:  $("#QueryType:checked", form).val(),
        CommodityId: $("#CommodityId", form).val(),
        ExcludeOutposts: $("#ExcludeOutposts", form).prop('checked'),
        SearchRange: $("#SearchRange", form).val()
        };

    $.ajax({
        url: '/api/EliteTradingTool/DataLists',
        type: 'POST',
        data: JSON.stringify(query),
        contentType: "application/json",
        success: function (data) {
            $("#DataSpinner").addClass("hidden");

            if (data.QueryType == 0) {
                var template = $.templates("#SystemOverviewTemplate");
            } else {
                var template = $.templates("#WantHaveTemplate");
            }
            var htmlOutput = template.render(data);
            $("#DataListing").html(htmlOutput);
            $("#DataListing").removeClass("hidden");
            $("#DataListTable").tablesorter({
                headers: {
                    0: {
                        sorter: 'distance'
                    },
                    3: {//zero-based column index
                        sorter: 'thousands'
                    },
                    4: {//zero-based column index
                        sorter: 'thousands'
                    },
                    5: {//zero-based column index
                        sorter: 'thousands'
                    }
                }
            });
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
    return false;
}





/* ***************** */
/* Twitter Typeahead */
/* ***************** */

// Credit http://twitter.github.io/typeahead.js/examples/
var substringMatcher = function (strs) {
    return function findMatches(q, cb) {
        var matches, substrRegex;

        // an array that will be populated with substring matches
        matches = [];

        // regex used to determine if a string contains the substring `q`
        substrRegex = new RegExp(q, 'i');

        // iterate through the pool of strings and for any string that
        // contains the substring `q`, add it to the `matches` array
        $.each(strs, function (i, str) {
            if (substrRegex.test(str)) {
                // the typeahead jQuery plugin expects suggestions to a
                // JavaScript object, refer to typeahead docs for more info
                matches.push({ value: str });
            }
        });

        cb(matches);
    };
};

var stationMapped = {};
function ApplyTypeahead() {
    var stationLabels = [];

    //$.ajax({
    //    url: 'api/EliteTradingTool/Stations',
    //    type: 'GET',
    //    data: { marketsOnly: true },
    //    success: function (data) {
    //        $.each(data, function (i, item) {
    //            stationMapped[item.System + " (" + item.Station + ")"] = item;
    //            stationLabels.push(item.System + " (" + item.Station + ")");
    //        });

    //        $('#TradeCalcForm #StartStation').typeahead({
    //            hint: true,
    //            highlight: true,
    //            minLength: 1
    //        }, {
    //            name: 'systemList',
    //            displayKey: 'value',
    //            source: substringMatcher(stationLabels)
    //        }).bind('typeahead:selected', function (obj, datum) {
    //            var selected = stationMapped[datum.value];
    //            userData.LastStationName = selected.Station;
    //            userData.LastSystemName = selected.System;
    //            userData.LastSystemAndStationName = datum.value;
    //            $("#RoutePlannerForm #StartSystem").typeahead('val', userData.LastSystemName);
    //            $("#SearchForm #CurrentLocation").typeahead('val', userData.LastSystemName);
    //        }).bind('typeahead:autocompleted', function (obj, datum) {
    //            var selected = stationMapped[datum.value];
    //            userData.LastStationName = selected.Station;
    //            userData.LastSystemName = selected.System;
    //            userData.LastSystemAndStationName = datum.value;
    //            $("#RoutePlannerForm #StartSystem").typeahead('val', userData.LastSystemName);
    //            $("#SearchForm #CurrentLocation").typeahead('val', userData.LastSystemName);
    //        });

    //        $('#TradeCalcForm #EndStation').typeahead({
    //            hint: true,
    //            highlight: true,
    //            minLength: 1
    //        }, {
    //            name: 'systemList',
    //            displayKey: 'value',
    //            source: substringMatcher(stationLabels)
    //        }).bind('typeahead:selected', function (obj, datum) {
    //            $("#TradeCalcForm #EndStationName").val(stationMapped[datum.value].Station);
    //        }).bind('typeahead:autocompleted', function (obj, datum) {
    //            $("#TradeCalcForm #EndStationName").val(stationMapped[datum.value].Station);
    //        }).on('blur', function () {
    //            if ($("#TradeCalcForm #EndStation").typeahead('val') == "") {
    //                $("#TradeCalcForm #EndStationName").val(null);
    //                if ($("#ReverseBtn").hasClass("hidden") == false)
    //                    $("#ReverseBtn").addClass("hidden");
                    
    //                $("#TradeCalcForm #MaxDistanceFromJumpInGroup").removeClass("hidden");
    //                $("#TradeCalcForm #SearchRangeGroup").removeClass("hidden");
                    
    //            } else {
    //                if ($("#ReverseBtn").hasClass("hidden"))
    //                    $("#ReverseBtn").removeClass("hidden");
    //                $("#TradeCalcForm #MaxDistanceFromJumpInGroup").addClass("hidden");
    //                $("#TradeCalcForm #SearchRangeGroup").addClass("hidden");
    //            }
    //        });

    //        $("#TradeCalcForm #StartStationName").val(userData.LastStationName);
    //        $("#TradeCalcForm #StartStation").typeahead('val', userData.LastSystemAndStationName);
    //    },
    //    error: function (data) {
    //        alert(d.responseText);
    //    }
    //});


    var systemsData = new Bloodhound({
        name: 'systemsData',
        limit: 15,
        remote: {
            url: '/api/EliteTradingTool/GetStarData?query=%QUERY',
            filter: function (data) {
                systemLabels = [];
                $.each(data, function (i, item) {
                    systemLabels.push({ name: item.Name });
                });
                return systemLabels;
            }
        },
        datumTokenizer: function (d) {
            return Bloodhound.tokenizers.whitespace(d.val);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace
    });
    systemsData.initialize();

    var lastChangedCalcSystem;

    $('#TradeCalcForm .systemList').typeahead({
        hint: true,
        highlight: true,
        minLength: 2
    }, {
        name: 'systemsData',
        displayKey: 'name',
        source: systemsData.ttAdapter()
    }).bind('typeahead:selected', function (obj, datum) {
        GetStationsInSystem(obj.target.id,datum.name);
        lastChangedCalcSystem = datum.name;
    }).on("blur", function (obj) {
        var value = $(this).typeahead('val');
        if (value == "") {
            $("#" + obj.target.id + "Stations").html('');
            $("#" + obj.target.id + "StationsGroup").addClass("hidden");
            CheckCalcSubmit(obj.target.id, true);
            if (obj.target.id == "StartSystem") {
                userData.StartSystem = "";
                userData.StartStationId = null;
            } else {
                userData.EndSystem = "";
                userData.EndStationId = null;
            }
            
        } else {
            if (value != lastChangedCalcSystem) {
                GetStationsInSystem(obj.target.id, value);
            }
            if (obj.target.id == "StartSystem") {
                userData.StartSystem = value;
            } else {
                userData.EndSystem = value;
            }
        }
    });

    $('#SearchForm .systemList').typeahead({
        hint: true,
        highlight: true,
        minLength: 2
    }, {
        name: 'systemsData',
        displayKey: 'name',
        source: systemsData.ttAdapter()
    });

    $('#FindTradesForm .systemList').typeahead({
        hint: true,
        highlight: true,
        minLength: 2
    }, {
        name: 'systemsData',
        displayKey: 'name',
        source: systemsData.ttAdapter()
    });

    $('#DataListsForm .systemList').typeahead({
        hint: true,
        highlight: true,
        minLength: 2
    }, {
        name: 'systemsData',
        displayKey: 'name',
        source: systemsData.ttAdapter()
    });

    $('#RareTradesForm .systemList').typeahead({
        hint: true,
        highlight: true,
        minLength: 2
    }, {
        name: 'systemsData',
        displayKey: 'name',
        source: systemsData.ttAdapter()
    });

    $('#MissingStationForm .systemList').typeahead({
        hint: true,
        highlight: true,
        minLength: 2
    }, {
        name: 'systemsData',
        displayKey: 'name',
        source: systemsData.ttAdapter()
    });

    $("#TradeCalcForm #StartSystem").typeahead('val', userData.StartSystem);
    $("#TradeCalcForm #EndSystem").typeahead('val', userData.EndSystem);
}



/* ******************** */
/*     Notification     */
/* ******************** */
function GetNotification() {
    $.ajax({
        url: '/api/EliteTradingTool/GetNotification',
        type: 'GET',
        contentType: "application/json",
        success: function (data) {
            if (data != null) {
                if (data.Message != null) {
                    CheckNotification(data.CreatedDate, data.Message);
                }
            }
        }
    });
}

function CheckNotification(date, message) {
    if (supports_html5_storage()) {
        // Get the data
        var data = localStorage["EliteTradingTool_Notification_Seen"];
        // Check if there is any data
        if (typeof data == 'undefined') {
            // No data
            ShowNotification(date, message);
            return true;
        }
        // Parse the data
        notificationData = JSON.parse(data);
        // See if the id is the same as the current and if its been seen previously
        if (notificationData.Date != date) {
            // Data but old message
            ShowNotification(date, message);
        } else if (notificationData.Date == date && !notificationData.Seen) {
            // Data and current message but not seen
            ShowNotification(date, message);
        }
    } else {
        ShowNotification(date, message);
    }
}

function ShowNotification(date, message) {
    $("#Notification .close").on("click", CloseNotification);
    $("#Notification .message").html(message);
    $("#Notification").removeClass("hidden");
    if (supports_html5_storage()) {
        localStorage["EliteTradingTool_Notification_Seen"] = JSON.stringify({ Date: date, Seen: false });
    }
}

function CloseNotification() {
    if (supports_html5_storage()) {
        // Get the data
        var data = localStorage["EliteTradingTool_Notification_Seen"];
        // Check if there is any data
        if (typeof data != 'undefined') {
            notificationData = JSON.parse(data);
            localStorage["EliteTradingTool_Notification_Seen"] = JSON.stringify({ Date: notificationData.Date, Seen: true });
        }
    }
    // Hide the notification
    $("#Notification").addClass("hidden");
}




/******************************************/
/*       Missing Stations Form            */
/******************************************/
function SetUpMissingStations() {
    ApplyTypeahead();

    $("#MissingStationForm").validate({
        submitHandler: MissingStations,
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

function MissingStations() {
    var querydata = {
        searchRange: $("#MissingStationForm #SearchRange:checked").val(),
        currentLocation: $("#MissingStationForm #CurrentLocation").typeahead('val')
    };

    ShowMissingStationsSpinner();

    $.ajax({
        url: 'MissingStations',
        type: 'POST',
        data: JSON.stringify(querydata),
        contentType: "application/json",
        success: function (data) {
            // Show/hide stuff
            $("#MissingStationListing").removeClass("hidden");

            // Display the search results
            MissingStationsResults(data);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function MissingStationsResults(data) {
    $("#MissingStationForm #MissingStationSpinner").addClass("hidden");

    // Check if there were any results
    if (data != null) {
        if (data.length > 0) {
            var dataProcessed = [];
            for (var i = 0; i < data.length; i++) {
                var currentItem = data[i];
                var item = {
                    Distance: currentItem.Distance,
                    Message: currentItem.Message
                }
                dataProcessed.push(item);
            }


            // Set the template data
            var MissingStationsListing = {
                Results: dataProcessed
            };


            // Configure and render the template
            var template = $.templates("#MissingStationListingTemplate");
            var htmlOutput = template.render(MissingStationsListing);
            $("#MissingStationListing").html(htmlOutput);

        } else {
            $("#MissingStationListing").html("<hr style='margin-top:10px' /><p>No results returned.</p>");
        }
    }
}




/* ******************** */
/* New Rank Achieved    */
/* ******************** */
function NewRankAchieved(rank, img) {
    $.amaran({
        content: {
            img: '/Content/images/' + img,
            user: rank + ' Rank Achieved',
            message: "You have achieved a new rank and unlocked new admin features.<br />You'll need to log out and back in again for this to take effect.<br />Check your profile page for details."
        },
        theme: 'user grey',
        closeButton: true,
        sticky: true
    });
};
