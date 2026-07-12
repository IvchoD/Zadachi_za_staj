const form = document.querySelector(".auth-form");

form.addEventListener("submit", async function (e) {

    e.preventDefault();

    const inputs = form.querySelectorAll("input");

    const data = {

        email: inputs[0].value,

        password: inputs[1].value

    };
    console.log(data);
    const response = await fetch("http://localhost:5251/api/users/login", {

        method: "POST",

        headers: {

            "Content-Type": "application/json"

        },

        body: JSON.stringify(data)

    });

    const result = await response.json();

    if (response.ok) {

        alert("Добре дошли, " + result.firstName + "!");

        window.location.href = "index.html";

    }
    else {

        alert(result.message);

    }

});