(function () {

    var RGX_EMAIL = /^[\w-]+@([\w-]+\.)+[\w-]+$/;
    var RGX_PSWD = /^.{8,150}$/;


    $(function () { $("#Email").focus(); });


    registerInputValidator($("#Email"), RGX_EMAIL);
    registerInputValidator($("#Password"), RGX_PSWD);


    window.setInterval(function () {

        if ($("#Email").attr('data-isValid') === 'true' && $("#Password").attr('data-isValid') === 'true') {
            $("#Email").removeAttr("disabled");
        }
        else {
            $("#Email").attr("disabled", "disabled");
        }

    }, 100);


})();
