(function () {



    function GetDataObj() {

        return {
            Name: $("#txt_Name").val(),
            Description: $("#txt_Description").val()
        };

    }



    function SendData(data) {


        $("#btn_CreateClub").attr("disabled", "disabled");
        $("html").css({ cursor: "wait" });


        var jsonData = JSON.stringify(data);

        var recapVal = grecaptcha.getResponse();



        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "/uhubapi/schoolClubs/Create",
            headers: {
                "g-recaptcha-response": recapVal
            },
            data: jsonData,
            complete: function () {

                $("#btn_CreateClub").removeAttr("disabled");
                $("html").css({ cursor: "default" });

            },
            success: function (data) {

                Name: $("#txt_Name").val("");
                Description: $("#txt_Description").val("");

                alert("Club Created");

                window.location.href = "/SchoolClub/" + data;
            },
            error: function (data) {
                alert(data.responseJSON);
            }
        });
    }




    $("#btn_CreateClub").click(function () {
        var data = GetDataObj();
        SendData(data);
    });



    validateInputElement($("#txt_Name"), /^(([ \u00c0-\u01ffA-z0-9'\-])+){3,100}$/);
    validateInputElement($("#txt_Description"), /^.{0,2000}$/);

})();