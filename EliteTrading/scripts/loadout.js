
/******************************************/
/*          Loadout Scripts               */
/******************************************/
/* ********** */
/* Paper Doll */
/* ********** */
var shipData;
function DoPaperDoll(data) {
    var target = $("#Ship");
    // Set the ship name and store the data
    $(target).html("<h3>" + data.Ship.Name + "</h3>");


    // Generate the slots
    GenerateWeaponSlots(target, "Class 2 Slot", "weapon", 2, data.Ship.C2);
    GenerateWeaponSlots(target, "Class 4 Slot", "weapon", 4, data.Ship.C4);
    GenerateWeaponSlots(target, "Class 6 Slot", "weapon", 6, data.Ship.C6);
    GenerateWeaponSlots(target, "Class 8 Slot", "weapon", 8, data.Ship.C8);
    GenerateModuleSlots(target, "Utility Slot", "utility", data.Ship.Utility);
    GenerateModuleSlots(target, "Support Slot", "support", data.Ship.Support);

    // Find the light alloy bulkhead
    var lightAlloy;
    for (var i = 0; i < data.Bulkheads.length; i++) {
        if (data.Bulkheads[i].Name == 'Lightweight Alloy') {
            lightAlloy = data.Bulkheads[i];
        }
    }

    // Generate the bulkhead slot and assign the light alloy details
    $('<div>' + lightAlloy.Name + '</div>')
        .data({ slotType: 'bulkhead', moduleId: lightAlloy.Id, weight: 0 })
        .prop('id', 'bulkhead')
        .addClass("bulkheadSlot")
        .addClass("slot")
        .appendTo($(target))
        .droppable({
            accept: function (d) {
                if (d.data("slotType") == 'bulkhead') {
                    return true;
                }
            },
            hoverClass: 'hovered',
            drop: SlotDrop
        });

    // Store the data
    var readout = $.templates("#ReadoutTemplate");
    shipData = {
        shipId: data.Ship.Id,
        name: data.Ship.Name,
        cost: data.Ship.Cost,
        maxClass: data.Ship.MaxClass,
        shipValue: accounting.formatNumber(data.Ship.Cost),
        cargo: data.Ship.Cargo,
        loadoutValue: 0,
        totalValue: accounting.formatNumber(data.Ship.Cost),
        insuranceCost: accounting.formatNumber(data.Ship.Cost * 0.05),
        emptyWeight: data.Ship.EmptyWeight,
        loadoutWeight: 0,
        cargoSpace: data.Ship.Cargo,
        emptyDistance: 0,
        fullDistance: 0,
        fuelTank: data.Ship.FuelTank,
        fuelCost: data.Ship.FSDMaxFuel,
        coefficient: data.Ship.FuelCoefficient,
        fuelPower: data.Ship.FuelPower,
        optimisedMass: data.Ship.OptimisedMass,
        selectedCargo: 0,
        jumpDistance: 0,
    };
    var html = readout.link("#Readout", shipData);
    $("#CargoSlider").slider({
        min: 0,
        max: data.Ship.Cargo,
        slide: function (event, ui) {
            $.observable(shipData).setProperty("selectedCargo", ui.value);
            $.observable(shipData).setProperty("mass", shipData.cargolessMass + ui.value);
            var jumpDistance = accounting.toFixed((Math.pow((shipData.fuelCost / shipData.coefficient), (1 / shipData.fuelPower)) * (shipData.optimisedMass / (shipData.cargolessMass + ui.value))), 2);

            $.observable(shipData).setProperty("jumpDistance", jumpDistance);
        }
    });
}

function GenerateWeaponSlots(container, title, type, weaponClass, numberOfSlots) {
    // Loop for the number of slots for this class
    for (i = 1; i <= numberOfSlots; i++) {
        // Generate the slot
        $('<div>' + title + '</div>')
            .data({ slotType: type, weaponclass: weaponClass, moduleId: 0, weight: 0 })
            .prop('id', type + 'C' + weaponClass + '_' + i + "Slot")
            .addClass(type + "Slot")
            .addClass("slot")
            .appendTo($(container))
            .droppable({
                accept: function (d) {
                    // Make sure only the right weapon classes can be dropped
                    if (d.data("weaponclass") <= weaponClass) {
                        return true;
                    }
                },
                over: function (event, ui) {

                    // Enable all the .droppable elements
                    $('.droppable').droppable('enable');

                    // If the droppable element we're hovered over already contains a .draggable element, 
                    // don't allow another one to be dropped on it
                    if ($(this).has('.clone').length) {
                        $(this).droppable('disable');
                    }
                },
                hoverClass: 'hovered',
                drop: SlotDrop
            });
    }
}

function GenerateModuleSlots(container, title, type, numberOfSlots) {
    // Loop for the number of slots for this module type
    for (i = 1; i <= numberOfSlots; i++) {
        // Generate the slot
        $('<div>' + title + '</div>')
            .data({ slotType: type, moduleId: 0, weight: 0 })
            .prop('id', type + '_' + i + "Slot")
            .addClass(type + "Slot")
            .addClass("slot")
            .appendTo($(container))
            .droppable({
                accept: function (d) {
                    if (d.data("slotType") == type) {
                        return true;
                    }
                },
                over: function (event, ui) {

                    // Enable all the .droppable elements
                    $('.droppable').droppable('enable');

                    // If the droppable element we're hovered over already contains a .draggable element, 
                    // don't allow another one to be dropped on it
                    if ($(this).has('.clone').length) {
                        $(this).droppable('disable');
                    }
                },
                hoverClass: 'hovered',
                drop: SlotDrop
            });
    }
}


/* ********** */
/* Modules    */
/* ********** */
function DoModules(data) {
    GenerateModuleTypeButtons("#ModuleTypeBtns");
    GenerateWeaponList("#Weapons", "Weapons", "weapon", data);
    GenerateModuleList("#Utilities", "Utility Modules", "utility", data.Utilities);
    GenerateModuleList("#Supports", "Support Modules", "support", data.Supports);
    GenerateModuleList("#Bulkheads", "Bulkheads", "bulkhead", data.Bulkheads);
}

function GenerateModuleTypeButtons(container) {
    $(container).html("");

    $("<a class='btn btn-weapons'>Weapons</a>").on("click", { newPanel: "Weapons" }, ShowPanel).appendTo(container);
    $("<a class='btn btn-utilities'>Utility</a>").on("click", { newPanel: "Utilities" }, ShowPanel).appendTo(container);
    $("<a class='btn btn-supports'>Support</a>").on("click", { newPanel: "Supports" }, ShowPanel).appendTo(container);
    $("<a class='btn btn-bulkheads'>Armor</a>").on("click", { newPanel: "Bulkheads" }, ShowPanel).appendTo(container);
}

function GenerateWeaponClassButtonsAndContainers(container, data) {
    $(container).html("<div id='WeaponClassBtns'></div>");
    var weaponClassBtns = $("#WeaponClassBtns");
    if (data.C2 > 0) {
        $("<a class='btn btn-default'>1</a>").on("click", { newPanel: "C1" }, ShowPanel).appendTo(weaponClassBtns);
        $("<a class='btn btn-default'>2</a>").on("click", { newPanel: "C2" }, ShowPanel).appendTo(weaponClassBtns);
    }
    if (data.C4 > 0) {
        $("<a class='btn btn-default'>3</a>").on("click", { newPanel: "C3" }, ShowPanel).appendTo(weaponClassBtns);
        $("<a class='btn btn-default'>4</a>").on("click", { newPanel: "C4" }, ShowPanel).appendTo(weaponClassBtns);
    }
    if (data.C6 > 0) {
        $("<a class='btn btn-default'>5</a>").on("click", { newPanel: "C5" }, ShowPanel).appendTo(weaponClassBtns);
        $("<a class='btn btn-default'>6</a>").on("click", { newPanel: "C6" }, ShowPanel).appendTo(weaponClassBtns);
    }
    if (data.C8 > 0) {
        $("<a class='btn btn-default'>7</a>").on("click", { newPanel: "C7" }, ShowPanel).appendTo(weaponClassBtns);
        $("<a class='btn btn-default'>8</a>").on("click", { newPanel: "C8" }, ShowPanel).appendTo(weaponClassBtns);
    }

    if (data.C2 > 0) {
        $("<div id='C1'></div>").appendTo(container);
        $("<div id='C2' class='hidden'></div>").appendTo(container);
    }
    if (data.C4 > 0) {
        $("<div id='C3' class='hidden'></div>").appendTo(container);
        $("<div id='C4' class='hidden'></div>").appendTo(container);
    }
    if (data.C6 > 0) {
        $("<div id='C5' class='hidden'></div>").appendTo(container);
        $("<div id='C6' class='hidden'></div>").appendTo(container);
    }
    if (data.C8 > 0) {
        $("<div id='C7' class='hidden'></div>").appendTo(container);
        $("<div id='C8' class='hidden'></div>").appendTo(container);
    }
}

function GenerateWeaponList(elementName, title, type, data) {
    var container = $(elementName);

    GenerateWeaponClassButtonsAndContainers(elementName, data.Ship);
    for (i = 0; i < data.Weapons.length; i++) {
        $('<div>' + data.Weapons[i].DisplayName + " (" + data.Weapons[i].Weight + "t) " + accounting.formatNumber(data.Weapons[i].Cost) + 'cr</div>')
            .data({ slotType: type, weaponclass: data.Weapons[i].Class, id: data.Weapons[i].Id, name: data.Weapons[i].DisplayName, weight: data.Weapons[i].Weight, cost: data.Weapons[i].Cost })
            .prop('id', type + '_' + data.Weapons[i].Id)
            .addClass(type + "Module")
            .addClass("module")
            .appendTo($("#C" + data.Weapons[i].Class))
            .draggable({ cursor: 'move', helper: 'clone', revert: "invalid" });
    }
}

function GenerateModuleList(elementName, title, type, data) {
    var container = $(elementName);
    $(container).html("");
    for (i = 0; i < data.length; i++) {
        $('<div>' + data[i].Name + " (" + data[i].Weight + "t) " + accounting.formatNumber(data[i].Cost) + 'cr</div>')
            .data({ slotType: type, id: data[i].Id, name: data[i].Name, weight: data[i].Weight, cost: data[i].Cost })
            .prop('id', type + '_' + data[i].Id)
            .addClass(type + "Module")
            .addClass("module")
            .appendTo($(container))
            .draggable({ cursor: 'move', helper: 'clone', revert: "invalid" });
    }
}



/* ************* */
/* Interactivity */
/* ************* */
var currentMainPanel = "Weapons";
var currentSubPanel = "C1";

function ResetPanels() {
    if (currentMainPanel != "Weapons") {
        $("#" + currentMainPanel).addClass("hidden");
    }
    if (currentSubPanel != "C1") {
        $("#" + currentSubPanel).addClass("hidden");
    }
    currentMainPanel = "Weapons";
    currentSubPanel = "C1";
    $("#" + currentMainPanel).removeClass("hidden");
    $("#" + currentSubPanel).removeClass("hidden");
}

function ShowPanel(event) {
    var newPanel = event.data.newPanel;
    if (newPanel.charAt(0) == "C") { // SubPanel
        $("#" + currentSubPanel).addClass("hidden");
        $("#" + newPanel).removeClass("hidden");
        currentSubPanel = newPanel;
    } else { // MainPanel
        $("#" + currentMainPanel).addClass("hidden");
        $("#" + newPanel).removeClass("hidden");
        currentMainPanel = newPanel;
    }

}

var counter = 0;
function SlotDrop(event, ui) {
    var cloneId = "moduleClone" + counter;
    var pos = $(this).position();
    var clone = $(ui.draggable).clone(); // clone the draggable so we keep a copy
    clone.detach().css({ position: 'absolute', top: pos.top, left: pos.left }).addClass("clone").appendTo(this); // position the clone at the upper left of its spot

    // set some of the data we need to check to determine whether we’re dealing with a clone or a new icon
    clone.attr("id", cloneId);
    clone.data('weight', $(ui.draggable).data("weight"));
    clone.data('cost', $(ui.draggable).data("cost"));
    clone.data('itemId', $(ui.draggable).data("id"));
    clone.data('slotType', $(ui.draggable).data("slotType"));
    clone.data('isClone', true);
    clone.data('slotId', $(this).prop("id"));

    // We need to make the clone into a new draggable so that it can be re-positioned.
    $("#" + cloneId).draggable({
        cursor: 'move',
        revert: 'invalid',
        drop: PageDrop
    });

    counter++;
    CalculateShipValues();
}

function PageDrop(event, ui) {
    $(ui.helper).remove(); //destroy clone
    $(ui.draggable).remove(); //remove from list
    CalculateShipValues();
}

function CalculateShipValues() {
    var loadoutWeight = 0;
    var cost = 0;
    $(".clone", $("#Ship")).each(function (index, element) {
        loadoutWeight += $(this).data("weight");
        cost += $(this).data("cost");
    });

    //$.observable(shipData).setProperty("loadoutWeight", loadoutWeight);
    //$.observable(shipData).setProperty("emptyWeight", $("#Ship").data("EmptyWeight"));


    FSDMaxFuel = shipData.fuelCost;
    fuelCoefficient = shipData.coefficient;
    fuelPower = shipData.fuelPower;
    optimisedMass = shipData.optimisedMass;

    var cargo = shipData.cargo;
    var mass = shipData.emptyWeight + loadoutWeight;
    var totalCost = shipData.cost + cost;
    var emptyDistance = accounting.toFixed((Math.pow((FSDMaxFuel / fuelCoefficient), (1 / fuelPower)) * (optimisedMass / mass)), 2);
    var fullDistance = accounting.toFixed((Math.pow((FSDMaxFuel / fuelCoefficient), (1 / fuelPower)) * (optimisedMass / (mass + cargo))), 2);

    $.observable(shipData).setProperty("loadoutValue", accounting.formatNumber(cost));
    $.observable(shipData).setProperty("totalValue", accounting.formatNumber(totalCost));
    $.observable(shipData).setProperty("insuranceCost", accounting.formatNumber(totalCost * 0.05));
    $.observable(shipData).setProperty("cargolessMass", mass);
    $.observable(shipData).setProperty("mass", mass);
    $.observable(shipData).setProperty("emptyDistance", emptyDistance);
    $.observable(shipData).setProperty("fullDistance", fullDistance);
    $.observable(shipData).setProperty("jumpDistance", emptyDistance);
}

function HookUpLoadoutSave() {

    $("#SaveBtn").click("on", function () {

        //Gather the data to send
        var loadout = new Object();
        loadout.ShipId = parseInt(shipData.shipId);
        loadout.LoadoutWeight = shipData.loadoutWeight;
        loadout.EmptyWeight = shipData.emptyWeight;
        loadout.LoadoutValue = parseInt(shipData.loadoutValue.replace(/,/g, ''));
        loadout.TotalValue = parseInt(shipData.totalValue.replace(/,/g, ''));
        loadout.InsuranceCost = parseInt(shipData.insuranceCost.replace(/,/g, ''));
        loadout.EmptyDistance = parseFloat(shipData.emptyDistance);
        loadout.FullDistance = parseFloat(shipData.fullDistance);

        loadout.ShipFittings = [];
        $(".clone", $("#Ship")).each(function (index, element) {
            loadout.ShipFittings.push({
                "Id": 0,
                "LoadoutId": 0,
                "ItemId": parseInt($(this).data("itemId")),
                "SlotType": $(this).data("slotType"),
                "SlotId": $(this).data("slotId")
            });
        });
        data = JSON.stringify(loadout);
        // Send it
        $.ajax({
            url: '/Loadout/Save',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: data,
            success: function (data) {
                $("#ResultMessage").html("Loadout saved.").addClass("text-success").delay(2000).queue(function () {
                    $(this).removeClass("text-success").dequeue();
                });
            }
        });
    });

}

function RestoreLoadout(loadoutData) {
    $.ajax({
        url: '/Loadout/LoadoutOptions',
        type: 'POST',
        data: { shipId: $("#ShipId").val() },
        success: function (data) {
            DoPaperDoll(data);
            DoModules(data);
            var mass = loadoutData.EmptyWeight + loadoutData.LoadoutWeight;
            $.observable(shipData).setProperty("cargolessMass", mass);
            $.observable(shipData).setProperty("mass", mass);
            $.observable(shipData).setProperty("loadoutWeight", loadoutData.LoadoutWeight);
            $.observable(shipData).setProperty("emptyWeight", loadoutData.EmptyWeight);
            $.observable(shipData).setProperty("loadoutValue", accounting.formatNumber(loadoutData.LoadoutValue));
            $.observable(shipData).setProperty("totalValue", accounting.formatNumber(loadoutData.TotalValue));
            $.observable(shipData).setProperty("insuranceCost", accounting.formatNumber(loadoutData.InsuranceCost));
            $.observable(shipData).setProperty("emptyDistance", loadoutData.EmptyDistance);
            $.observable(shipData).setProperty("fullDistance", loadoutData.FullDistance);
            $.observable(shipData).setProperty("jumpDistance", loadoutData.EmptyDistance);
            // loop through the loadoutData.ShipFittings
            for (i = 0; i <= loadoutData.ShipFittings.length - 1; i++) {
                // Find the target slot
                var slot = $("#" + loadoutData.ShipFittings[i].SlotId);
                // Copy the module
                var module = $("#" + loadoutData.ShipFittings[i].SlotType + "_" + loadoutData.ShipFittings[i].ItemId);
                var cloneId = "moduleClone" + counter;
                var pos = $(slot).position();
                var clone = module.clone();  // clone the module so we keep a copy
                clone.detach().css({ position: 'absolute', top: pos.top, left: pos.left }).addClass("clone").appendTo($(slot)); // position the clone at the upper left of its spot

                // set some of the data we need to check to determine whether we’re dealing with a clone or a new icon
                clone.attr("id", cloneId);
                clone.data('weight', $(module).data("weight"));
                clone.data('cost', $(module).data("cost"));
                clone.data('itemId', $(module).data("id"));
                clone.data('slotType', $(module).data("slotType"));
                clone.data('isClone', true);
                clone.data('slotId', $(slot).prop("id"));

                // We need to make the clone into a new draggable so that it can be re-positioned.
                $("#" + cloneId).draggable({
                    cursor: 'move',
                    revert: 'invalid',
                    drop: PageDrop
                });

                counter++;
                // place the module
                // mark the slot as full
            }

        }
    });

}

