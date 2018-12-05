(function () {

    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);




    var simplemde = new SimpleMDE(
        {
            autosave: {
                enabled: false
            },
            element: document.getElementById("inputContent"),
            previewRender: function (plainText) {
                return mdConverter.makeHtml(plainText);
            },
            promptURLs: true,
            spellChecker: true,
            status: ["lines", "words", {
                className: "charCount",
                defaultValue: function (el) {
                    this.charCount = 0;
                    el.innerHTML = "Characters: 0 (10 - 10k)";
                },
                onUpdate: function (el) {
                    var ct = simplemde.value().length;
                    el.innerHTML = "Characters: " + ct + " (10 - 10k)";
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
            },
            success: function (data) {
                $("#inputTitle").val("");
                simplemde.value("");
                $("#inputMakePrivate")[0].checked = false;
                $("#inputCanComment")[0].checked = true;

                alert("Post Created");

                window.location.href = "/Post/" + data;
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


    validateInputElement($("#inputTitle"), /^.{1,100}$/);
    validateInputElement($("#inputContent"), /^.{10,10000}$/);


})();