var createUser = new Vue({
    el: "#userForm",
    data: {
        email: "",
        username: "",
        password: "",
        name: "",
        PhoneNumber: "",
        major: ""
    },
    methods: {
        readRefs() {
            let refs = this.$refs;
            this.username = refs.inputUSR.value;
            this.password = refs.inputPWD.value;
            this.email = refs.inputEmail.value;
            this.fullname = refs.inputFirst.value + refs.inputLast.value;
            this.major = refs.inputMajor.value;
            this.PhoneNumber = refs.inputPhone.value;
            this.Year = refs.inputYear.value;
            this.GradDate = refs.inputGrad.value;
            this.Company = refs.inputCompany.value;
            this.JobTitle = refs.inputJob.value;
            this.sendObj = JSON.stringify(this.$data);
            console.log(this.sendObj);
        }
    },
    watch: {
        sendObj: function() {
            $.ajax({
                method: "POST",
                url: "/uhubapi/account/createuser",
                data: createUser.sendObj,
            }).error(function() {
                alert("Error during user creation");
            });
        }
    }
});
/*
    When email is unfocused, sends ajax request to get list of majors to supply the dropdown
 */
$("#email").blur(function(){
    var majornames = [];
    var majorList = $.ajax({
        method: "GET",
        data: $("#email").valueOf()[0].value
    }).success(function() {
        for(var index=0; index<majorList.length; index++){
            if(majorList[index].IsEnabled){
                majornames.push(majorList[index].Name);
            }
        }

        $("#autocomplete").autocomplete({
            source: majornames,
        });
    });
});
