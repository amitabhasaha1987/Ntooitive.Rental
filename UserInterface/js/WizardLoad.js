$(function () {
    debugger;

    var url_Start = 'http://localhost:4865';
    debugger;
    document.createElement('x-sandiegouniontribune-search');
    document.createElement('x-sandiegouniontribune-cretifiedagents');
    document.createElement('x-sandiegouniontribune-featuredagents');
    document.createElement('x-sandiegouniontribune-classifiedlisting');
    var url = '';
    var data = '';
    var data_count = '';
    var property_type = '';
    

    //search wizard
    if ($('x-sandiegouniontribune-search').length != 0)
    {
        url = url_Start + '/wizard/Search';
        data = {};
        
        $.ajax({
            type: 'GET',
            url: url,
            contentType: 'text/plain',
            xhrFields: {
                withCredentials: false
            },
            headers: {
            },
            success: function (data) {
                $('x-sandiegouniontribune-search').html(data);
            },
            error: function () {
            }
        });
    }

    //cretifiedagents
    if ($('x-sandiegouniontribune-cretifiedagents').length != 0) {
        url = url_Start + '/wizard/CretifiedAgents';
        debugger;
        data_count = $('x-sandiegouniontribune-cretifiedagents').attr('data-count');
        data = {
            count: data_count
        };

        $.ajax({
            type: 'GET',
            url: url,
            contentType: 'text/plain',
            data : data,
            xhrFields: {
                withCredentials: false
            },
            headers: {
            },
            success: function (data) {
                $('x-sandiegouniontribune-cretifiedagents').html(data);
            },
            error: function () {
            }
        });
    }

    //featuredagents
    if ($('x-sandiegouniontribune-featuredagents').length != 0) {
        url = url_Start + '/wizard/FeaturedAgents';
        debugger;
        data_count = $('x-sandiegouniontribune-cretifiedagents').attr('data-count');
        data = {
            count: data_count
        };

        $.ajax({
            type: 'GET',
            url: url,
            contentType: 'text/plain',
            data: data,
            xhrFields: {
                withCredentials: false
            },
            headers: {
            },
            success: function (data) {
                $('x-sandiegouniontribune-featuredagents').html(data);
            },
            error: function () {
            }
        });
    }


    //classifiedlisting
    if ($('x-sandiegouniontribune-classifiedlisting').length != 0) {
        url = url_Start + '/wizard/ClassifiedListing';
        debugger;
        data_count = $('x-sandiegouniontribune-classifiedlisting').attr('data-count');
        list_type = $('x-sandiegouniontribune-classifiedlisting').attr('list-type');
        data = {
            count: data_count,
            type: list_type
        };

        $.ajax({
            type: 'GET',
            url: url,
            contentType: 'text/plain',
            data: data,
            xhrFields: {
                withCredentials: false
            },
            headers: {
            },
            success: function (data) {
                $('x-sandiegouniontribune-classifiedlisting').html(data);
            },
            error: function () {
            }
        });
    }


    //communitylisting
    if ($('x-sandiegouniontribune-communitylisting').length != 0) {
        url = url_Start + '/wizard/CommunityListing';
        debugger;
        var community = $('x-sandiegouniontribune-communitylisting').attr('community-name');
        list_type = $('x-sandiegouniontribune-communitylisting').attr('list-type');
        data = {
            communityName: community,
            type: list_type
        };

        $.ajax({
            type: 'GET',
            url: url,
            contentType: 'text/plain',
            data: data,
            xhrFields: {
                withCredentials: false
            },
            headers: {
            },
            success: function (data) {
                $('x-sandiegouniontribune-communitylisting').html(data);
            },
            error: function () {
            }
        });
    }
})