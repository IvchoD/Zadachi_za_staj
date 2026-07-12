const form = document.querySelector(".checkout-form");

form.addEventListener("submit", async function(e){

    e.preventDefault();

    const deliveryCompany = "Econt";

    const paymentMethod = "Cash on Delivery";

    const response = await fetch("http://localhost:5251/api/order",{

        method:"POST",

        headers:{
            "Content-Type":"application/json"
        },

        body:JSON.stringify({

            userId:1,

            deliveryCompany,

            paymentMethod

        })

    });

    const result = await response.json();

    if(result.success){

        window.location.href = "thankyou.html";

    }
    else{

        alert(result.message);

    }

});