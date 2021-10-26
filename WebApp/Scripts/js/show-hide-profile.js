function show_hide_profile(role) {
    $("#trainer-profile").addClass("d-none");
    $("#trainee-profile").addClass("d-none");
    if (role == "Trainer") {
        $("#trainer-profile").removeClass("d-none");
    }

    if (role == "Trainee") {
        $("#trainee-profile").removeClass("d-none");
    }
}

$('#Role').on('change', function () {
    show_hide_profile(this.value)
});

show_hide_profile($('#Role').val());
