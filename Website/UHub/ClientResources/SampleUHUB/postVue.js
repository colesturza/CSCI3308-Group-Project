new Vue({
  el: '#enterInfo',
  data: {
    title: "",
    content: "",
    public: "",
    comment: ""
  },
  methods: {
    readRefs:function(){
      this.data = this.$refs
      this.title = this.data.title.value
      this.content = this.data.content.value
      this.public = this.data.public.checked
      this.comment = this.data.comment.checked
      this.parentID = window.href
      this.postobj = JSON.stringify(this.$data)
      console.log(JSON.stringify(this.$data))
    }
  },
  watch: {
    postobj: $.ajax({
      method: "POST",
      data: this.postobj,
      url: "https://u-hub.life/uhubapi/posts/Create"
    })
  }
})
