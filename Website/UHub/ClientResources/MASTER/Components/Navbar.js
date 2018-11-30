(function () {

    var communities = [];
    var clubName = "Communities";


    Vue.component('navbar-component', {
        data: function () {
            $.ajax({
                method: "POST",
                url: "/uhubapi/schoolclubs/GetAllBySchool",
                async: false,
                success: function (data) {
                    var url = window.location.href;
                    var seperated = url.split('/');
                    var currCommID = seperated.slice(-1)[0];

                    communities = data;
                    communities.sort(dynamicSort("Name"));


                    var commLen = communities.length;
                    for (var i = 0; i < commLen; i++) {
                        if (currCommID == communities[i].ID) {
                            clubName = communities[i].Name;
                        }
                    }

                },
                error: function (error) {
                    console.log(error);
                }
            });
            return {
                clubName: clubName,
                communities: communities
            }
        },
        template:
            `
        <div >
            <div id="mainNav">
                <div class="logoWrapper">
                    <a href="/School" tabindex="-1">UHUB</a>
                </div>
                <nav>
                    <div id="navbarNavDropdown">
                        <ul class="navbar-nav mr-auto">
                            <li class="nav-item active dropdown">
                                <a class="nav-link dropdown-toggle"
                                    id="navbarDropdownMenuLink"
                                    aria-haspopup="true"
                                    aria-expanded="false">
                                    {{clubName}}
                                </a>
                            </li>
                        </ul>
                    </div>
                    <div id="navbarDropdownMenu" class="dropdown-menu-UHUB scrollable-menu" aria-labelledby="navbarDropdownMenuLink">
                        <a class="dropdown-item" href="/School/Clubs">-- View All --</a>
                        <a class="dropdown-item"
                            v-for="community in communities"
                            v-bind:href="'/SchoolClub/' + community.ID">
                            {{ community.Name }}
                        </a>
                    </div>
                    <div class="searchWrapper">
                        <input class="form-control" type="search" placeholder="Search" class="form-control" />
                        <div>
                            <button class="form-control" type="submit" class="btn btn-secondary">Search</button>
                            <a href="/Account" class="btn btn-secondary acctLnk">My Account</a>
                        </div>
                    </div>
                    
                </nav>
            </div>
        </div>
        `
    });

    new Vue({ el: "#navbar-uhub" });



    $("*:not(#navbarDropdownMenuLink, #navbarDropdownMenuLink *, #navbarDropdownMenu, #navbarDropdownMenu *)")
        .click(function () {
            $("#navbarDropdownMenu").removeClass("show");
        });


    $("#navbarDropdownMenuLink")
        .click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            $("#navbarDropdownMenu").toggleClass("show");
        });

})();
