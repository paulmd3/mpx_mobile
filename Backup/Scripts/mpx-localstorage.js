if (window.addEventListener) {
    window.addEventListener("storage", handle_storage, false);
}
else {
    window.attachEvent("onstorage", handle_storage);
};

$(function() {
    if (navigator.offLine) {
        $("#pageTitle").append("(Offline)");
        $("#status").append("(Offline)");
    }
    else if (navigator.onLine) {
        $("#pageTitle").append("(Online)");
    }
});


function handle_storage(e) {
    if (!e) { e = window.event; }
    console.dir(e);
}


var Bookmark = (function() {
    var Bookmark = function(id, last_name, first_name, address, phone, email, url) {
        this.id = id;
        this.last_name = last_name;
        this.first_name = first_name;
        this.address = address;
        this.phone = phone;
        this.email = email;
        this.url = url;
    };

    return function(id, last_name, first_name, address, phone, email, url) {
        return new Bookmark(id, last_name, first_name, address, phone, email, url);
    }
})();


var Bookmarks = {

    storage: 'mpx.bookmarks.v1',

    init: function() {
    },

    add: function(bookmark) {
        // @todo validate input

        s = this.getAll();

        console.log(s);
        
        s.bookmarks.push(bookmark);

        localStorage[this.storage] = JSON.stringify(s);
    },

    getAll: function() {
        // get the list of bookmarks from storage, or stub
        var bookmarks = localStorage[this.storage] || { "bookmarks": [] };
        return (typeof bookmarks == "string") ? JSON.parse(bookmarks) : bookmarks;
    },

    get: function(id) {
        var bookmarks = this.getAll();
        if (!bookmarks) return;

        var result = null;
        jQuery.each(bookmarks.bookmarks, function(index, obj) {
            if (id == obj.id) {
                result = obj;
            }
        });
        return result;
    },

    search: function() {
        //
    },

    list: function() {
        var b = this.getAll();
        console.log(b);

        $("#bookmarks_list_template").tmpl(b.bookmarks).appendTo("ul#donors");

    },

    details: function(id) {

        var bookmark = this.get(id);
        if (!bookmark)
            alert("not a valid bookmark");

        var div = $("#bookmark_details_template").tmpl(bookmark);

        $(".dim").show();
        $("#bookmark_details_template").tmpl(bookmark).appendTo("div.panel");

        // want to put this in a card above and put a closer next to it
        //$( "div.panel" ).html(div);
    }
};


$(function() {
    $("ul#donors li > a").live('click', function() {

        if ($(this).parent('li').attr('data-donor-id').length < 1)
            alert("no data id");
        var donor_id = $(this).parent('li').attr('data-donor-id');

        Bookmarks.details(donor_id);
    });
});

$(function() {

    if (!window.localStorage) { alert("device doesn't support local storage"); return false; }

    n = Bookmark('1', 'Anarde', 'Matthew', '24 south morgan, chicago, il 60607', '718.570.9260', 'anarde@gmail.com', 'http://anarde.me');
    o = Bookmark('2', 'Johnson', 'Sean', '806 forest ave, evanston, il 60202', '630.432.1278', 'sean.johnson@gmail.com', 'http://sean-johnson.com');

    //Bookmarks.add(n);
    //Bookmarks.add(o);
    //Bookmarks.list();

    console.log( localStorage[Bookmarks.storage] );
});
