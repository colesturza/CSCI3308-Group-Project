var communities = [];
Vue.component('navbar-component', {
    data: function () {
        $.ajax({
            method: "POST",
            url: "/uhubapi/schoolclubs/GetAllBySchool",
            success: function (data) {
                communities = data;
            },
            error: function (error) {
                console.log(error);
            }
        });
        return {
            communities
            console.log(communities);
        }
    },
    template: 
        `
        <div class="container">
            <div id="mainNav" class="container-fullwidth">
                <nav class= "navbar navbar-expand navbar-dark bg-dark">
                    <a class="navbar-brand" href="/School">UHUB</a>
                    <div class="collapse navbar-collapse" id="navbarNavDropdown">
                        <ul class="navbar-nav mr-auto">
                            <li class="nav-item active dropdown">
                                <a class="nav-link dropdown-toggle"
                                    id="navbarDropdownMenuLink"
                                    data-toggle="dropdown"
                                    aria-haspopup="true"
                                    aria-expanded="false">
                                    Communities
                                </a>
                                <div class="dropdown-menu scrollable-menu" aria-labelledby="navbarDropdownMenuLink">
                                    <a class="dropdown-item"
                                        v-for="community in communities"
                                        v-bind:href="'/SchoolClub/' + community.ID">
                                        {{ community.Name }}
                                    </a>
                                </div>
                            </li>
                        </ul>
                        <form class="form-inline">
                            <input class="form-control mx-2 my-auto d-inline"
                                type="search" placeholder="Search"
                                aria-label="Search">
                                <button class="btn btn-secondary my-2 my-md-0"
                                    type="submit">
                                    Search
                                </button>
                        </form>
                        <a href="/Account" class="btn btn-secondary my-2 my-md-0">My Account</a>
                    </div>
                </nav>
            </div>

            <div id="auxNav" class="container-fullwidth">
                <ul class="nav navbar-expand-md bg-light justify-content-center">
                    <li class="nav-item navbar-brand pl-3 pr-0 pt-1">
                        <span class="align-middle">Sort</span>
                    </li>
                    <li class="nav-item px-3 py-2">
                        <div class="dropdown">
                            <button class="btn btn-sm dropdown-toggle" type="button" id="sortDropdownButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                Select
                            </button>
                            <div class="dropdown-menu" aria-labelledby="sortDropdownButton">
                                <a class="dropdown-item" href="#">Popular</a>
                                <a class="dropdown-item" href="#">Date Posted</a>
                            </div>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
        `
})

new Vue({ el: "#navbar-uhub" })