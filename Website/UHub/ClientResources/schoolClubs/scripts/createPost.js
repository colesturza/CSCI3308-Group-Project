var url = window.location.href
var seperated = url.split('/')
var commentID = seperated.slice(-1)[0];
console.log(commentID)


new Vue({
    el: '#enterInfo',
    data: {
        Name: "",
        Content: "",
        IsPublic: "",
        CanComment: "",
        ParentID: ""
    },
    methods: {
        readRefs: function () {
            let refs = this.$refs;

            this.Name = refs.inputTitle.value;
            this.Content = refs.inputContent.value;
            this.IsPublic = !refs.inputMakePrivate.checked;
            this.CanComment = refs.inputCanComment.checked;
            this.ParentID = commentID;
            this.postobj = JSON.stringify(this.$data);
            console.log(JSON.stringify(this.$data));
        },
        sendData() {
            $("#btn_CreatePost").attr("disabled", "disabled");
            $("html").css({ cursor: "wait" });

            $.ajax({
                method: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: this.postobj,
                url: "/uhubapi/posts/Create",
                complete: function (data) {
                    $("#btn_CreatePost").removeAttr("disabled");
                    $("html").css({ cursor: "default" });
                    console.log(data);
                },
                success: function (data) {
                    console.log(data);
                    //alert(data);
                },
                error: function (data) {
                    console.log(data);
                    //alert(data.responseJSON.status);
                }
            })
        }
    }
})
