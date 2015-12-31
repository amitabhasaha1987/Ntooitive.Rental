function GalleriaSetUp(galleriaId) {
    var id = "#" + galleriaId;
    Galleria.loadTheme('/Content/js/galleria.classic.min.js');
    Galleria.configure({
        lightbox: true
    });    // Initialize Galleria
    Galleria.run(id);
    Galleria.ready(function () {
        this.bind('image', function (e) {
            e.imageTarget.alt = e.galleriaData.description;
        });
    });
}

function ShowMapView(divid, lat, lng, maptype) {
    debugger;
    var map = null;
    var pinInfoBox;  //the pop up info box
    var infoboxLayer = new Microsoft.Maps.EntityCollection();
    var pinLayer = new Microsoft.Maps.EntityCollection();
    var pinInfobox = null;

    $("#" + divid).html('');
   
    if (maptype === "birdeye") {
        map = new Microsoft.Maps.Map(document.getElementById(divid), {
            credentials: "Aj1jij2hUsqUbBkgNY5kkOlHc8uXc5ZlI332uFT9rUZ5eKg8oH4ePSSoa6u1gVAv",
            mapTypeId: Microsoft.Maps.MapTypeId.birdseye,
          
            center: new Microsoft.Maps.Location(lat, lng),
            zoom: 15,
            showBreadcrumb: false, 
            enableClickableLogo: false,
            enableSearchLogo: false,
            showDashboard: false,
            showMapTypeSelector: false,
            showScalebar: false,
            useInertia: false,
            disablePanning: false,
            disableZooming: false
        });

        pinInfobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false });
        infoboxLayer.push(pinInfobox);

        var location = new Microsoft.Maps.Location(lat, lng);
        var pushpin = new Microsoft.Maps.Pushpin(location, {
            icon: "/image/icons/apartment.png"
        });
        pinLayer.push(pushpin);
        map.entities.push(pinLayer);
        


    } else {
        map = new Microsoft.Maps.Map(document.getElementById(divid), {
            credentials: "Aj1jij2hUsqUbBkgNY5kkOlHc8uXc5ZlI332uFT9rUZ5eKg8oH4ePSSoa6u1gVAv",
            mapTypeId:  Microsoft.Maps.MapTypeId.road,
            center: new Microsoft.Maps.Location(lat, lng),
            zoom: 15,
            showBreadcrumb: false,
            enableClickableLogo: false,
            enableSearchLogo: false,
            showDashboard: false,
            showMapTypeSelector: false,
            showScalebar: false,
            useInertia: false,
            disablePanning: false,
            disableZooming: false
        });

        pinInfobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false });
        infoboxLayer.push(pinInfobox);

        var location = new Microsoft.Maps.Location(lat, lng);
        var pushpin = new Microsoft.Maps.Pushpin(location, {
            icon: "/image/icons/apartment.png"
        });
        pinLayer.push(pushpin);
        map.entities.push(pinLayer);

    }

    $('.MicrosoftMap').addClass('map_height');
}