var string1 = "https://u-hub.life/schoolclubs/21"
var seperated = string1.split('/')
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
      data: this.postobj,
      url: "/uhubapi/posts/Create"
    })
  }
})
