(function () {


    var jsonClubDataOld = null;
    var oldResponseErr = null;


    function GetDataObj() {

        return {
            Name: $("#txt_Name").val(),
            Description: $("#txt_Description").val()
        };

    }



    function SendData(data) {


        $("#btn_CreateClub").attr("disabled", "disabled");
        $("html").css({ cursor: "wait" });


        var jsonClubData = JSON.stringify(data);


        if (jsonClubData == jsonClubDataOld) {
            alert(oldResponseErr);
        }
        jsonClubDataOld = jsonClubData;




        if (!$("#txt_Name").val().match(/^(([ \u00c0-\u01ffA-z0-9'\-])+){3,100}$/)) {
            oldResponseErr = 'Club Name Invalid';
            alert(oldResponseErr);
            $("#btn_CreateClub").removeAttr("disabled");
            $("html").css({ cursor: "default" });
            return;
        }
        else if (!$("#txt_Description").val().match(/^.{0,2000}$/)) {
            oldResponseErr = 'Club Description Invalid';
            alert(oldResponseErr);
            $("#btn_CreateClub").removeAttr("disabled");
            $("html").css({ cursor: "default" });
            return;
        }




        var recapVal = grecaptcha.getResponse();



        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "/uhubapi/schoolClubs/Create",
            headers: {
                "g-recaptcha-response": recapVal
            },
            data: jsonClubData,
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
                oldResponseErr = data.responseJSON;
                alert(oldResponseErr);
            }
        });
    }




    $("#btn_CreateClub").click(function () {
        var data = GetDataObj();
        SendData(data);
    });



    registerInputValidator($("#txt_Name"), /^(([ \u00c0-\u01ffA-z0-9'\-])+){3,100}$/);
    registerInputValidator($("#txt_Description"), /^.{0,2000}$/);

})();