$(document).ready(function(){
  $(".form-group").hide();
  $(".btn-outline-primary").hide();
});

var profileView = new Vue({
  el: "#display-profile",
  data: {
    email: "",
    username: "",
    name: "",
    major: "",
    phoneNumber: "",
    year: "",
    gradDate: "",
    company: "",
    jobTitle: "",
    schoolId: ""
  },
  beforeMount: function(){
    var userReq = $.ajax({
      method: "POST",
      url: "/uhubapi/Users/GetMe",
      dataType: "json",
      statusCode: {
        200: function(data) {
          console.log(data)
        }
      }
    })
    .done(function(data){console.log(data)})
  }
})
