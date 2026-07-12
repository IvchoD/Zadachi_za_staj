const cartItems = document.getElementById("cartItems");
const cartSummary = document.getElementById("cartSummary");

// =================================
// Load Cart
// =================================

async function loadCart() {

    const response = await fetch("http://localhost:5251/api/cart/1");
    const data = await response.json();

    renderCart(data.items);

    const summaryResponse = await fetch("http://localhost:5251/api/cart/summary/1");
    const summary = await summaryResponse.json();

    cartSummary.innerHTML = `

<div class="summary-card">

    <h2>🧾 Обобщение</h2>

    <p>

        <span>Междинна сума</span>

        <strong>${summary.subtotal.toFixed(2)} €</strong>

    </p>

    <p>

        <span>Доставка</span>

        <strong>${summary.shipping == 0 ? "Безплатна" : summary.shipping.toFixed(2)+" €"}</strong>

    </p>

    <hr>

    <h3>

        <span>Общо</span>

        <strong>${summary.grandTotal.toFixed(2)} €</strong>

    </h3>

    <button
    class="checkout-btn"
    onclick="window.location.href='checkout.html'">

      Продължи към поръчка →

     </button>

</div>

`;
}

// =================================
// Render Cart
// =================================

function renderCart(items) {

    cartItems.innerHTML = "";

    items.forEach(item => {

        cartItems.innerHTML += `
        <div class="cart-item">

            <img src="images/products/${item.imagePath}" alt="${item.productName}">

            <div class="cart-info">

                <h3>${item.productName}</h3>

                <p class="cart-price">${item.price.toFixed(2)} €</p>

                <div class="cart-controls">

                    <button onclick="changeQuantity(${item.productId}, ${item.quantity - 1})">−</button>

                    <span>${item.quantity}</span>

                    <button onclick="changeQuantity(${item.productId}, ${item.quantity + 1})">+</button>

                </div>

                <p>Общо: ${item.total.toFixed(2)} €</p>

                <button class="remove-btn" onclick="removeItem(${item.productId})">

                  🗑 Премахни

                </button>

            </div>

        </div>
        `;

    });

}

// =================================
// Change Quantity
// =================================

async function changeQuantity(productId, quantity) {

    await fetch("http://localhost:5251/api/cart/quantity", {

        method: "PUT",

        headers: {
            "Content-Type": "application/json"
        },

        body: JSON.stringify({

            userId: 1,
            productId,
            quantity

        })

    });

    await loadCart();

}

// =================================
// Remove Item
// =================================

async function removeItem(productId) {

    await changeQuantity(productId, 0);

}

// =================================
// Start
// =================================

loadCart();