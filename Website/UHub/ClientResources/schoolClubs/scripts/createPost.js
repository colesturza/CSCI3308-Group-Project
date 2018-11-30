var url = window.location.href
var seperated = url.split('/')
console.log(seperated)
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
    readRefs:function(){
      this.data = this.$refs
      this.title = this.data.title.value
      this.content = this.data.content.value
      this.public = this.data.public.checked
      this.comment = this.data.comment.checked
      this.parentID = seperated.slice(-1)[0]
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
