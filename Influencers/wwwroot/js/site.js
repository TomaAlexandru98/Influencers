var numberOfStars = 5;
var yellowColor = "#E69A8DFF";
var mauveColor = "#5F4B8BFF";

function configRating(articleId) {
    if (checkIfCookieExistsForArticle(articleId)) {
        makeStarsFinal(articleId, getCookie(articleId), yellowColor);
    } else {
        $(`#article-rating-${articleId}`).on("mouseover", function () { addHoverForStars(articleId) });
        $(`#article-rating-${articleId}`).on("mouseout", function () { removeHoverForStars(articleId) });
        addPosibilyToVote(articleId);
    }
}

function addPosibilyToVote(articleId) {
    for (let i = 1; i <= numberOfStars; i++) {
        $(`#article-rating-${articleId} span:nth-child(${i})`).on("click", function () { sendVote(articleId, i) });
    }
}

function removeEventListenersForRating(articleId) {
    let rating = $(`#article-rating-${articleId}`);
    rating.unbind("mouseover");
    rating.unbind("mouseout");
    for (let i = 1; i <= numberOfStars; i++) {
        removeClickEventForStar(articleId, i);
    }
}

function removeClickEventForStar(articleId, childId) {
    let star = $(`#article-rating-${articleId} span:nth-child(${childId})`);
    star.unbind("click");
}

function sendVote(articleId, voteValue) {
    $.ajax({
        url: "/Article/Vote",
        method: "POST",
        data: { articleId: articleId, voteValue: voteValue },
        success: function () {
            createCookie(articleId, voteValue);
            removeEventListenersForRating(articleId);
            makeStarsFinal(articleId, voteValue, yellowColor);
        }
    });
}

var makeStarsFinal = function (articleId, childId, color) {
    for (let i = 1; i <= childId; i++) {
        $(`#article-rating-${articleId} span:nth-child(${i})`).css("color", color);
    }
};

function addHoverForStars(articleId) {
    for (let i = 1; i <= numberOfStars; i++) {
        $(`#article-rating-${articleId} span:nth-child(${i})`).on("mouseover", () => { makeStarsFinal(articleId, i, yellowColor) });

    }
}

function removeHoverForStars(articleId) {
    for (let i = 1; i <= numberOfStars; i++) {
        $(`#article-rating-${articleId} span:nth-child(${i})`).on("mouseout", makeStarsFinal(articleId, i, mauveColor));
    }
}

function checkIfCookieExistsForArticle(articleId) {
    if (typeof (getCookie(articleId)) !== 'undefined') return true;
    return false;
}

function createCookie(articleId, voteValue) {
    $.cookie(`${articleId}`, `${voteValue}`, { path: '/', secure: true });
}

function getCookie(articleId) {
    return $.cookie(`${articleId}`);
}

function createTagElement(tag) {
    let div = document.createElement("div");
    let li = document.createElement("li");
    let span = document.createElement("span");
    let i = document.createElement("i");

    span.style.flex = "9";
    span.innerHTML = tag;

    i.setAttribute("class", "fa fa-remove");
    i.style.cursor = "pointer";
    i.style.color = "#5F4B8BFF";
    i.style.cursor = "pointer";
    i.style.margin = "3px 0";
    i.addEventListener("click", function () {
        div.remove();
        var index = tags.indexOf(tag);
        tags.splice(index, 1);
    });
    i.addEventListener("mouseover", function () {
        i.style.color = "#E69A8DFF";
    });
    i.addEventListener("mouseout", function () {
        i.style.color = "#5F4B8BFF";
    });

    div.style.display = "flex";
    div.style.width = "13rem";
    div.append(li);
    div.append(span);
    div.append(i);

    $("#ulTags").append(div);
}

function addOptionDataList(datalistId, value) {
    var option = document.createElement("option");
    option.value = value;
    $(datalistId).append(option);
}

function addTag(tag) {
    let tagIsNull = $("#tagIsNull");
    let tagIsAlreadyAdded = $("#tagIsAlreadyAdded");

    if (tag == "") {
        if (tagIsAlreadyAdded.css("display") != "none") tagIsAlreadyAdded.css("display", "none");
        if (tagIsNull.css("display") == "none") tagIsNull.css("display", "block");
        return;
    }
    if (tagIsNull.css("display") != "none") tagIsNull.css("display", "none");

    if (tags.indexOf(tag) == -1) {
        if (tagIsNull.css("display") != "none") tagIsNull.css("display", "none");
        if (tagIsAlreadyAdded.css("display") != "none") tagIsAlreadyAdded.css("display", "none");
        tags.push(tag);
        createTagElement(tag);
        return;
    }
    tagIsAlreadyAdded.html(`Tag ${tag} is already added.`);
    tagIsAlreadyAdded.css("display", "block");
}

function convertStringToArray(articleTags) {
    while (articleTags != "") {
        let indexOfComma = articleTags.indexOf(",");
        if (indexOfComma != -1) {
            let substring = articleTags.substring(0, indexOfComma);
            tags.push(substring);
            substring += ",";
            articleTags = articleTags.replace(substring, "");
        } else {
            tags.push(articleTags);
            return;
        }
    }
}

function createTagElementsFromArray(articleTags) {
    if (articleTags != "") {
        convertStringToArray(articleTags);
        for (let i = 0; i < tags.length; i++) {
            createTagElement(tags[i]);
        }
    }
}

function getAllTagsForDatalist() {
    $.ajax({
        url: "/Article/GetAllAsJson",
        success: function (response) {
            sendDataToDatalist("#tags-nav-bar", response);
        }
    });
}

function getAllAuthorsForDatalist() {
    $.ajax({
        url: "/Author/GetAllAsJson",
        success: function (response) {
            sendDataToDatalist("#authors-nav-bar", response);
        }
    });
}

function sendDataToDatalist(datalistId, response) {
    for (let i = 0; i < response.length; i++) {
        addOptionDataList(datalistId, response[i]);
    }
}