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


        var recapVal = grecaptcha.getResponse();



        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "/uhubapi/posts/Create",
            headers: {
                "g-recaptcha-response": recapVal
            },


            data: data,
            complete: function () {


                $("#btn_CreateClub").removeAttr("disabled");
                $("html").css({ cursor: "default" });

            },
            success: function (data) {

                console.log(data);

                alert(data);

                if (data.canLogin === true) {
                    window.location.href = "/";
                }
                else {
                    window.location.href = "/Account/Confirm/New";
                }
            },
            error: function (data) {
                alert(data.responseJSON.status);
            }
        });
    }




    $("#btn_CreateClub").click(function () {
        var data = GetDataObj();
        SendData(data);
    });



})();