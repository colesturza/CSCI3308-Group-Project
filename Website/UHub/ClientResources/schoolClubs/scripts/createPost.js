var url = window.location.href
var seperated = url.split('/')
var commentID = seperated.slice(-1)[0];
console.log(commentID)


new Vue({
  el: '#enterInfo',
  data: {
    title: "",
    content: "",
    public: "",
    comment: "",
    parentID: ""
  },
  methods: {
      readRefs: function () {
      let refs = this.$refs;
      
      this.title = refs.title.value
      this.content = refs.content.value
      this.public = refs.public.checked
      this.comment = refs.comment.checked
      this.parentID = commentID
      this.postobj = JSON.stringify(this.$data)
      console.log(JSON.stringify(this.$data))
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
