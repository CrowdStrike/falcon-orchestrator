function successMsg(message) {
    return "<div class=\"alert alert-success\">" +
                 "<a href=\"#\" class=\"close\" data-dismiss=\"alert\"><i class=\"fa fa-close\"></i></a>" +
                 "<i class=\"fa fa-check-square-o\"></i> <b>Success!</b> " + message + "</div>";
}

function failureMsg(message) {
    return "<div class=\"alert alert-danger\">" +
                 "<a href=\"#\" class=\"close\" data-dismiss=\"alert\"><i class=\"fa fa-close\"></i></a>" +
                 "<i class=\"fa fa-exclamation-triangle\"></i> <b>Error!</b> " + message + "</div>";
}

function escapeCharacters(myid) {
    return "#" + myid.replace(/(:|\.|\[|\]|,)/g, "\\$1");
}
