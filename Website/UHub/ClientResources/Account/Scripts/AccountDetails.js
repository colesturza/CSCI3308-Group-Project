
getUser = new Vue({
	el: "#user-info",
	data: {
		user: {}
	},
	methods: {
		user_get() {
			this.user = user
		},
	},
	beforeMount() {
		this.user_get()
	}
});