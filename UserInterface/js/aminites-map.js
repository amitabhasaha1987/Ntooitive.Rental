/// <reference path="utility.js" />
var map = null;
var pinInfoBox;  //the pop up info box
var infoboxLayer = new Microsoft.Maps.EntityCollection();
var pinLayer = new Microsoft.Maps.EntityCollection();
var pinInfobox = null;
var amenities_data = null;

function plotAmenitiesMap(mlsId, enityid, mapid, address, maxDis, house_lat, house_long) {
    
    map = new Microsoft.Maps.Map(document.getElementById(mapid), {
        credentials: "Aj1jij2hUsqUbBkgNY5kkOlHc8uXc5ZlI332uFT9rUZ5eKg8oH4ePSSoa6u1gVAv",
        mapTypeId: Microsoft.Maps.MapTypeId.road,
        height: 450,
        width: 970,
        enableSearchLogo: false
    });
    pinInfobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false });
    infoboxLayer.push(pinInfobox);

    if (enityid == "") {
        var enityids = "3578,4013,4170,4581,5400,5540,5800,6000,7011,8060,8211,9221,9530,9996,9545";
        GetAmilityDetails(mlsId, enityids, address, maxDis, house_lat, house_long);
    } else {
        GetAmilityDetails(mlsId, enityid, address, maxDis, house_lat, house_long);
    }
  

}
function displayInfobox(e) {
    pinInfobox.setOptions({
        width: 300,
        height: 120, title: e.target.Title, description: e.target.Description, visible: true, offset: new Microsoft.Maps.Point(0, 25)
    });
    pinInfobox.setLocation(e.target.getLocation());
}
function hideInfobox(e) {
    pinInfobox.setOptions({ visible: false });
}
function GetAmilityDetails(mlsId, enityid, address, distance, house_lat, house_long) {
    var getObj = localStorage.getObj(mlsId + "_" + "amenities");
    if (getObj == null) {
        SetPlot(mlsId, enityid, address, distance, house_lat, house_long);
    } else {
        var today = new Date();
        var validUpto = new Date(getObj[1].validUpto);
        if (today > validUpto) {
            localStorage.removeItem(mlsId + "_" + "amenities");
            SetPlot(mlsId, enityid, address, distance, house_lat, house_long);
        } else {
            amenities_data = getObj[0].amenities_data;
            SetMap(house_lat, house_long);
        }
    }
}
function SetMap(house_lat, house_long) {
    var locations = [];
    var location = new Microsoft.Maps.Location(house_lat, house_long);
    var pushpin = new Microsoft.Maps.Pushpin(location, {
        icon: "/image/icons/apartment.png"
    });
    pinLayer.push(pushpin);
    map.entities.push(pinLayer);
    locations.push(location);

    $.each(amenities_data, function (index, value) {
    var location = new Microsoft.Maps.Location(value.Latitude, value.Longitude);
        var pushpin = new Microsoft.Maps.Pushpin(location, {
            icon: "/image/icons/" + value.EntityTypeID + ".png"
        });
        pushpin.Title = value.DisplayName; //usually title of the infobox
        var distance = getMiles(value.__Distance);
        pushpin.Description = "<span style='font-weight:bold;'>" + value.AddressLine + "," + value.AdminDistrict2 + "," + value.AdminDistrict + "<br/> Call : " + value.Phone
            + "<br/>" + "Distance from Prop : " + distance + " miles"; //information you want to display in the infobox
        Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', displayInfobox);
        Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', hideInfobox);

        pinLayer.push(pushpin);

        Microsoft.Maps.Events.addHandler(map, 'viewchange', hideInfobox);
        map.entities.push(pinLayer);
        map.entities.push(infoboxLayer);

        locations.push(location);
        var bestview = Microsoft.Maps.LocationRect.fromLocations(locations);
        if (locations.length == 1) {
            map.setView({ center: locations[0], zoom: 15 });
        } else {
            map.setView({ bounds: bestview });
        }

    });

}
function SetPlot(mlsId, enityid, address, distance, house_lat, house_long) {
    $.getJSON("https://spatial.virtualearth.net/REST/v1/data/" +
          "f22876ec257b474b82fe2ffcb8393150/NavteqNA/NavteqPOIs?" +
          "spatialFilter=nearby('" + address + "'," + distance + ")" +
          "&$filter=EntityTypeID in" +
          "(" + enityid + ")&$select=DisplayName,Latitude," +
          "Longitude,EntityTypeID,AddressLine,AdminDistrict2" +
          ",AdminDistrict,Phone,__Distance&$top=25&key=" +
          "Aj1jij2hUsqUbBkgNY5kkOlHc8uXc5ZlI332uFT9rUZ5eKg8oH4ePSSoa6u1gVAv" +
          "&$format=json&jsonp=?", function (data) {
            var tomorrow = new Date();
            tomorrow.setDate(tomorrow.getDate() + 1);
            amenities_data = data.d.results;
            var amenitiesDetails = [
                { "amenities_data": data.d.results },
                { "validUpto": tomorrow }
            ];
            localStorage.setObj(mlsId+"_"+"amenities", amenitiesDetails);
            SetMap(house_lat, house_long);
        });
}

