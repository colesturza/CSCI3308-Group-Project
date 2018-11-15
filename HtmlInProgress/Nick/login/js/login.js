var login;

var loginReq = $.ajax({
    method: "POST",
    url: "#https://u-hub.life/uhubapi/auth/GetToken",
    data: login.sendObj
}).success(function() {
    console.log("Successful Login");
});

login = new Vue({
    el: "#userForm",
    data: {},
    methods: {
        readRefs() {
            let refs = this.$refs;
            this.email = refs.inputEmail.value;
            this.password = refs.inputPWD.value;
            this.sendObj = JSON.stringify(this.$data);
            console.log(this.sendObj);
        },
    },
    watch: {
        sendObject: loginReq
    }
});