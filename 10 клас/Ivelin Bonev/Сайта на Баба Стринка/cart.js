const CART_KEY = 'baba-strinka-cart';
const EUR_RATE = 1.95583;

const PRODUCTS = {
    djanka: {
        id: 'djanka',
        name: 'Класическа джанка',
        priceBgn: 12.50,
        image: 'pictures/djanki-indrishe-kompot.webp'
    },
    cheresha: {
        id: 'cheresha',
        name: 'Бутикова череша с костилка',
        priceBgn: 15.00,
        image: 'pictures/big_kompot-ot-chereshi_snimka_5.webp'
    },
    mushmula: {
        id: 'mushmula',
        name: 'Екзотична мушмула',
        priceBgn: 18.99,
        image: 'pictures/sladko_ot_Mushmula.webp'
    },
    praskova: {
        id: 'praskova',
        name: 'Прасковата на дядо ти Христо',
        priceBgn: 22.00,
        image: 'pictures/kompotpraskovki2-flx.webp'
    },
    dulja: {
        id: 'dulja',
        name: 'Носталгична дюля',
        priceBgn: 14.00,
        image: 'pictures/kompotcheduli.webp'
    },
    yagoda: {
        id: 'yagoda',
        name: 'Специална ягода',
        priceBgn: 16.50,
        image: 'pictures/kompot-ot-iagodi-8342d06a41740c279a4701afb76cbc58-[101345].webp'
    }
};

function toEur(bgn) {
    return (bgn / EUR_RATE).toFixed(2);
}

function formatPrice(bgn) {
    return `${bgn.toFixed(2)} лв. / ${toEur(bgn)} €`;
}

function getCart() {
    try {
        return JSON.parse(localStorage.getItem(CART_KEY)) || {};
    } catch {
        return {};
    }
}

function saveCart(cart) {
    localStorage.setItem(CART_KEY, JSON.stringify(cart));
    window.dispatchEvent(new Event('cart-updated'));
}

function getCartCount() {
    const cart = getCart();
    return Object.values(cart).reduce((sum, qty) => sum + qty, 0);
}

function addToCart(productId, amount = 1) {
    const cart = getCart();
    cart[productId] = (cart[productId] || 0) + amount;
    saveCart(cart);
}

function changeQuantity(productId, delta) {
    const cart = getCart();
    const next = (cart[productId] || 0) + delta;
    if (next <= 0) {
        delete cart[productId];
    } else {
        cart[productId] = next;
    }
    saveCart(cart);
}

function removeFromCart(productId) {
    const cart = getCart();
    delete cart[productId];
    saveCart(cart);
}

function clearCart() {
    saveCart({});
}

function playAddToCartAnimation(btn) {
    btn.classList.remove('animate');
    void btn.offsetWidth;
    btn.classList.add('animate');
    setTimeout(() => btn.classList.remove('animate'), 750);
}

function getCartItems() {
    const cart = getCart();
    return Object.entries(cart)
        .filter(([, qty]) => qty > 0)
        .map(([id, qty]) => ({
            product: PRODUCTS[id],
            qty
        }))
        .filter((item) => item.product);
}

function getCartTotals() {
    const items = getCartItems();
    const totalBgn = items.reduce((sum, item) => sum + item.product.priceBgn * item.qty, 0);
    return { totalBgn, totalEur: toEur(totalBgn), items };
}

function updateCartBadge() {
    const badge = document.getElementById('cart-count');
    if (!badge) return;
    const count = getCartCount();
    badge.textContent = count;
    badge.hidden = count === 0;
}

function initProductCards() {
    document.querySelectorAll('.product-card[data-product-id]').forEach((card) => {
        const productId = card.dataset.productId;
        const addBtn = card.querySelector('.add-to-cart');

        addBtn.addEventListener('click', () => {
            addToCart(productId, 1);
            playAddToCartAnimation(addBtn);
        });
    });
}

document.addEventListener('DOMContentLoaded', () => {
    updateCartBadge();
    window.addEventListener('cart-updated', updateCartBadge);
    if (document.querySelector('.product-card[data-product-id]')) {
        initProductCards();
    }
});
