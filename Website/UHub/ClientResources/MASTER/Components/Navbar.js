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
