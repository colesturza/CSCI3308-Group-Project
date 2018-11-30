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
    }
  },
  watch: {
    postobj: $.ajax({
        method: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        complete: function (data) {
            console.log(data);
        },
      data: this.postobj,
      url: "/uhubapi/posts/Create"
    })
  }
})
