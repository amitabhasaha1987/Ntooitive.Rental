var schools_details = null;
function callApi(divid, mlsId, state, zip, address, city, schoolType, levelCode) {

    $.ajax({
        url: "/api/GetNearbySchools?state=" + state + "&zip=" + zip + "&address=" + address + "&city=" + city + "&schoolType=" + schoolType + "&levelCode=" + levelCode
    }).done(function (data) {
        var tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        var schoolDetails = [
            { "schools_data": data },
            { "validUpto": tomorrow }
        ];
        localStorage.setObj(mlsId + "_" + levelCode + "_" + "schools", schoolDetails);
        schools_details = data;

        setSchooldData(divid);
        //gsRating

    });
}
function setSchooldData(divid) {
    $.each(schools_details, function (index, value) {
        var gsRatingUrl = GetUrl(value.gsRating);
        var parentRatingUrl = GetUrl(value.parentRating);
        var placetoPut = '#' + divid + ' > tbody:last';
        var district;
        if (value.district == null) {
            district = '';
        } else {
            district = value.district;
        }
        $(placetoPut).append(
            '<tr><td class="school_name">' + value.name + ' <span class="district">' + district + '</span></td>' +
            '<td align="center" class="distance">' + value.distance +' mi</td>' +
            '<td align="center" class="grades">' + value.gradeRange +'</td>' +
            '<td align="center" class="stu_tea">' + '<img src="' + parentRatingUrl + '" /></td>' +
            '<td align="center" class="rating">' + '<img src="' + gsRatingUrl + '" /></td>' +
            '</tr>');
    });

}
function GetUrl(rating) {
    if (rating >= 1 && rating <= 3) {
        return "/image/red/" + rating + ".png";
    } else if (rating >= 4 && rating <= 5) {
        return "/image/yellow/" + rating + ".png";
    } else if (rating >= 6 && rating <= 10) {
        return "/image/green/" + rating + ".png";
    } else if (rating == 0  ) {
            return "/image/NR.png";
    }
}

function GetSchools(divid,mlsId, state, zip, address, city, schoolType, levelCode) {
    var apiKey = "eliwqg0t8cnrqwvsyh0tcaxx";
    var radius = 5;
    var limit = 3;

    var getObj = localStorage.getObj(mlsId + "_" +levelCode+"_" +"schools");
    if (getObj == null) {
        callApi(divid,mlsId, state, zip, address, city, schoolType, levelCode);
    } else {
        var today = new Date();
        var validUpto = new Date(getObj[1].validUpto);
        if (today > validUpto) {
            localStorage.removeItem(mlsId + "_" + levelCode + "_" + "schools ");
            callApi(divid,mlsId, state, zip, address, city, schoolType, levelCode);
        } else {
            schools_details = getObj[0].schools_data;
            setSchooldData(divid);
        }
    }
}



