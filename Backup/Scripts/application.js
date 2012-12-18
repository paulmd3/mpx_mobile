
jQuery.fn.log = function(msg) {
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

window.applicationCache.addEventListener('checking',logEvent,false);
window.applicationCache.addEventListener('noupdate',logEvent,false);
window.applicationCache.addEventListener('downloading',logEvent,false);
window.applicationCache.addEventListener('cached',logEvent,false);
window.applicationCache.addEventListener('updateready',logEvent,false);
window.applicationCache.addEventListener('obsolete',logEvent,false);
window.applicationCache.addEventListener('error',logEvent,false);

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

window.addEventListener('load', function(e) {
    if (window.applicationCache) {
        window.applicationCache.addEventListener('updateready', function(e) {
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


///////////////////
// database
///////////////////

function errorHandler(transaction, error)
{
    // error.message is a human-readable string.
    // error.code is a numeric error code
    alert('Oops.  Error was '+error.message+' (Code '+error.code+')');
 
    // Handle errors here
    var we_think_this_error_is_fatal = true;
    if (we_think_this_error_is_fatal) return true;
    return false;
}

function nullDataHandler(transaction, results) { }

// The first thing we want to do is create the local
// database (if it doesn't exist) or open the connection
// if it does exist.
var databaseOptions = {
    fileName: "sqlite1_MPX",
    version: "1.0",
    displayName: "SQLite1 MPX",
    maxSize: 1024
};



// Now that we have our database properties defined, let's
// create or open our database, getting a reference to the
// generated connection.
//
try {
    if(window.openDatabase)
    {
        database = openDatabase(
            databaseOptions.fileName,
            databaseOptions.version,
            databaseOptions.displayName,
            databaseOptions.maxSize
         );
         if(! database)
         {
            alert("Failed to open the database on disk.");
         }
    }
} catch (e) {}

// -------------------------------------------------- //
// -------------------------------------------------- //

// According to Safari, all
// queries must be part of a transaction. To execute a
// transaction, we have to call the transaction() function
// and pass it a callback that is, itself, passed a reference
// to the transaction object.
database.transaction(
    function(transaction) {
        // Create our history10 table if it doesn't exist.
        transaction.executeSql(
            "CREATE TABLE IF NOT EXISTS history10 ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 'ts2' TEXT NOT NULL, 'data' TEXT NOT NULL,'url' TEXT NOT NULL );"
        );
        
        // should generally match bookmarks in the server-side cache
        // should update on bookmarkeddonors page load 
        transaction.executeSql(
            "CREATE TABLE IF NOT EXISTS donors ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 'last_name' TEXT NOT NULL, 'first_name' TEXT NOT NULL, 'email' TEXT NULL, 'address' TEXT NULL, 'phone' TEXT NULL);"
        );
           
});


////////////////
// history
////////////////
var pagehistoryiPhone = [];
var pagePhone = 0;
var lastPage = "";

/// save html history in pagehistory array
function saveInHistory(url) 
{
    if (!url) alert('url is null');

    var hashed = $.md5(url.toString());
    pagehistoryiPhone.push(hashed);
    
    pagePhone = ++pagePhone;
    
    console.log('pagehistoryiPhone:' + pagehistoryiPhone);
    console.log('pagePhone:' + pagePhone);
}

function getPage(url) {
    console.log("getPage(): " + url);
    // put the hash in the history stack
    saveInHistory(url);
    console.log("last page: " + lastPage);
    
    var md5 = $.md5(url);
    // now get the url and drop it in the db w/ the dt
    $("#loading").css("display", "block");

    $.get(url.toString(), null, function(data) {
        var date = new Date();
        var current = date.getDate();
        database.transaction(function(transaction) {
            transaction.executeSql("INSERT INTO history10 (ts2,data,url) VALUES (?,?,?)", [current, $("#page").html(), md5]);
        });

        var last = document.location.href.toString()
        var last = $.md5(last);
        lastPage = last;

        $("#page").html(data);
        $("#page").scrollTop(0);

        $("#loading").css("display", "none");
    });
}

function backPage(url) {
    if (pagePhone > 0) {
        console.log(pagehistoryiPhone);
        
        var hash = pagehistoryiPhone[pagehistoryiPhone.length - 2];
        console.log(hash);

        loadPageWithHash(hash);
        
        pagehistoryiPhone.pop();
        //pagehistoryiPhone.push(hash);
    }
}

$(function() {

    $('a').live('click', function(e) {
        var elem_id = $(this).attr("id");
        var a = $(this);
        var href = $(this).attr("href");

        if (a.is('.forward, .newforward')) {
            getPage(href);
        }
        else if (a.is(".postforward")) {
            document.location = href;
        }
        else if (a.is(".mailforward")) {
            window.location.replace = href;
        }
        else if (a.is(".backwards")) {
            backPage();
        }
        else if (a.hasClass("clear")) {
            database.transaction(function(transaction) {
                transaction.executeSql("delete from history10 ");
            });
            alert("nuking db");
            document.location = '/Home/DefaultMenu';
        }

        return false;
    });
});


function loadPageWithHash(hash) {
    console.log('attempting to load page w/ ' + hash);
    if (!database) {
        alert('Oops.  Database doesn\'t exist');
    }
    else {
        database.transaction(
            function(transaction) {
        transaction.executeSql('select * from history10 where url = ? order by ts2 desc limit 1;', [hash], function(transaction, results) {
                    console.log(results.rows);
                    if (results.rows.length > 0) {
                        var page = results.rows.item(0);
                        //console.log('loading page w/ ' + page.data);
                        $("#page").html(page.data);
                    }
                });
            });
    }
    return false;
}




// should check to see if the user exists using something unique
// prob the server-side ID
function insertDonor(first, last, phone, email, address) 
{
    if (!database) {
        alert('Oops.  Database doesn\'t exist');
    }
    else {
        database.transaction(
            function(transaction) {
                transaction.executeSql('insert into donors (first_name, last_name, email, address, phone) VALUES ("' + first + '", "' + last + '", "'+ email +'", "'+ address +'", "'+ phone +'");', [], nullDataHandler, errorHandler);
            }
        );
    }
}