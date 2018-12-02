(function () {

    var simplemde = new SimpleMDE(
        {
            element: document.getElementById("inputContent"),
            autosave: {
                enabled: false
            },
            promptURLs: true,
            spellChecker: true,
            status: ["lines", "words", {
                className: "charCount",
                defaultValue: function (el) {
                    this.charCount = 0;
                    el.innerHTML = "0 Characters (10 - 10k)";
                },
                onUpdate: function (el) {
                    var ct = simplemde.value().length;
                    el.innerHTML = ct + " Characters (10 - 10k)";
                }
            }]
        });



    var url = window.location.href;
    var seperated = url.split('/');
    var clubID = seperated.slice(-1)[0];


    function getData() {

        return {
            Name: $("#inputTitle").val(),
            Content: simplemde.value(),
            IsPublic: !$("#inputMakePrivate")[0].checked,
            CanComment: $("#inputCanComment")[0].checked,
            ParentID: clubID
        };
    }

    function sendData(formData) {
        $("#btn_CreatePost").attr("disabled", "disabled");
        $("html").css({ cursor: "wait" });

        var jsonData = JSON.stringify(formData);

        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            url: "/uhubapi/posts/Create",
            complete: function (data) {
                $("#btn_CreatePost").removeAttr("disabled");
                $("html").css({ cursor: "default" });
                console.log(data);
            },
            success: function (data) {
                $("#inputTitle").val("");
                simplemde.value("");
                $("#inputMakePrivate")[0].checked = false;
                $("#inputCanComment")[0].checked = true;

                alert(data);
            },
            error: function (data) {
                alert(data.responseJSON);
            }
        });
    }


    $("#btn_CreatePost").click(function () {
        var data = getData();
        sendData(data);
    });

})();