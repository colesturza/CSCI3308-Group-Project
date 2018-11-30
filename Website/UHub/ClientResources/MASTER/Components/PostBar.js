Vue.component('postbar-component', {
    template:
        `
 <div id="auxNav" class="container-fullwidth">
    <ul class="nav navbar-expand-md bg-light">
        <li class="w-uhub-offset">
        <li>
        <li class="nav-item">
            <div class="text-center">
                <button class= "mt-2">
                    Create Post
                </button>
            </div>
        </li>
        <li class="w-15">
        </li>
        <li class="nav-item navbar-brand pl-3 pr-0 pt-1">
            <span class="align-middle">Sort</span>
        </li>
        <li class="nav-item px-3 py-2 col-sm-1">
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
        `
})

new Vue({ el: "#postbar-uhub" })