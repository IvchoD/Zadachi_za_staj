const API_KEY = "aGZdTHlBQDVGyZkyrZ0AS3qmqn796SRbFClrOJx5";

async function loadPicture() {

    const date = document.getElementById("date").value;

    if (date === "") {
        return;
    }

    const today = new Date().toISOString().split("T")[0];

    if (date > today) {

        document.getElementById("error").textContent =
        "Газовата уредба на машината на времето гръмна. Моля, изберете минала дата!";

        return;

    }

    document.getElementById("error").textContent = "";

    try {

        const response = await fetch(
            `https://api.nasa.gov/planetary/apod?api_key=${API_KEY}&date=${date}`
        );

        const data = await response.json();

        if (!response.ok) {

            document.getElementById("error").textContent =
            data.error?.message || "Възникна грешка.";

            return;

        }

        document.getElementById("title").textContent = data.title;

        const image = document.getElementById("image");

        if (data.media_type === "image") {

            image.style.display = "block";
            image.src = data.url;
            image.alt = data.title;

            document.getElementById("description").textContent =
            data.explanation;

        }

        else {

            image.style.display = "none";

            document.getElementById("description").innerHTML =
            `За тази дата NASA е публикувала видео.<br><br>
            <a href="${data.url}" target="_blank">
                ▶ Гледай видеото
            </a>`;

        }

    }

    catch(error){

        document.getElementById("error").textContent =
        "Грешка при свързване с NASA API.";

        console.log(error);

    }

}