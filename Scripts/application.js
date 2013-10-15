
jQuery.fn.log = function (msg) {
    console.log("%s: %o", msg, this);
    return this;
};

function logEvent(event) {
    console.log(event.type);
}

function is_online() {
    if (navigator.onLine) {
        alert('online')
    } else {
        alert('offline');
    }
}

window.applicationCache.addEventListener('checking', logEvent, false);
window.applicationCache.addEventListener('noupdate', logEvent, false);
window.applicationCache.addEventListener('downloading', logEvent, false);
window.applicationCache.addEventListener('cached', logEvent, false);
window.applicationCache.addEventListener('updateready', logEvent, false);
window.applicationCache.addEventListener('obsolete', logEvent, false);
window.applicationCache.addEventListener('error', logEvent, false);

function cacheStatus() {
    var appCache = window.applicationCache;

    appCache.update();

    switch (appCache.status) {
        case appCache.UNCACHED: // UNCACHED == 0
            return 'UNCACHED';
            break;
        case appCache.IDLE: // IDLE == 1
            return 'IDLE';
            break;
        case appCache.CHECKING: // CHECKING == 2
            return 'CHECKING';
            break;
        case appCache.DOWNLOADING: // DOWNLOADING == 3
            return 'DOWNLOADING';
            break;
        case appCache.UPDATEREADY:  // UPDATEREADY == 4
            return 'UPDATEREADY';
            break;
        case appCache.OBSOLETE: // OBSOLETE == 5
            return 'OBSOLETE';
            break;
        default:
            return 'UKNOWN CACHE STATUS';
            break;
    };
}

window.addEventListener('load', function (e) {
    if (window.applicationCache) {
        window.applicationCache.addEventListener('updateready', function (e) {
            if (window.applicationCache.status == window.applicationCache.UPDATEREADY) {
                // Browser downloaded a new app cache.
                // Swap it in and reload the page to get the new hotness.
                window.applicationCache.swapCache();
                if (confirm('A new version of this site is available. Load it?')) {
                    window.location.reload();
                }
            } else {
                // Manifest didn't change. Nothing new on server.
            }
        }, false);
    }
}, false);



// history

var History = window.History; // Note: We are using a capital H instead of a lower h
if (!History.enabled) { }

// Bind to StateChange Event
History.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
    var State = History.getState(); // Note: We are using History.getState() instead of event.state
    History.log(State.data, State.title, State.url);
});

var _history = ["/Dashboard/Index"];

function _getPage(url) {
    $("#loading").css("display", "block");
    
    History.pushState(null, null, url);
    _history.push(url);
    alert();
    $.get(url.toString(), null, function (data) {
        $("#page").html(data);
        $("#page").scrollTop(0);
        $("#loading").css("display", "none");
    });
}

// @todo error checking
function _back() {

    var url = _history[_history.length - 2]; // account for the zero
    alert(_history.length);
    alert(_history[_history.length - 1]);
    if (url == undefined) {
        url = "/Dashboard/Index";
    }
    $("#loading").css("display", "block");
    _history.pop();
    _history.pop();
    alert(url);
    $.get(url, null, function (data) {

        History.pushState(null, null, url);

        $("#page").html(data);
        $("#page").scrollTop(0);

        $("#loading").css("display", "none");
    });
}

$(function () {

//    alert(_history.length);

//    $('a').bind('click', function (e) {
//        var elem_id = $(this).attr("id");
//        var a = $(this);
//        var href = $(this).attr("href");

//        if (a.is('.forward, .newforward')) {
//            alert(href);
//            _getPage(href);
//        }
//        else if (a.is(".postforward")) {
//            document.location = href;
//        }
//        else if (a.is(".mailforward")) {
//            window.location.replace = href;
//        }
//        else if (a.is(".backwards")) {
//            _back();
//        }

//        return false;
//    });
});