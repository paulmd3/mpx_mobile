$(function () {
    $(document).keydown(function (e) {
        var ev = document.all ? window.event : e;
        if (ev.keyCode == 13) {
            searchDonors();
        }
    });
});
function Validate() {
    
    if ($('#txtEmail').val() == ''
            && $('#txtAccount').val() == ''
            && $('#txtOrg').val() == ''
            && $('#txtName').val() == ''
            && $('#txtZip').val() == '') {
        alert('Enter a term to search.');
        return false;
    }
    var regEmail = /\w@\w*\.\w/;
    var regZip = /^[0-9]{5}$/;
    var regAccount = /^\d/;

    if ($('#txtEmail').val() != ''
        && !regEmail.test($('#txtEmail').val())) {
        alert("Enter a valid email address.");
        return false;
    }

    if ($('#txtAccount').val() != ''
        && !regAccount.test($('#txtAccount').val())) {
        alert("Enter a numeric value.");
        return false;
    }

    if ($('#txtZip').val() != ''
        && !regAccount.test($('#txtZip').val())) {
        alert("Enter a numeric value.");
        return false;
    }

    return true;
}