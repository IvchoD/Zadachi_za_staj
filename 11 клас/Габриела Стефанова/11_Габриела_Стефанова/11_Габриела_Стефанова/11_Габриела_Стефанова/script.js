 const NASA_API_KEY = 'JpD5G25GfOMhxyofjWQcnzBUYol8ANmCpYWYCBYP';
 
const dateInput   = document.getElementById('date-input');
  const form        = document.getElementById('time-machine-form');
  const errorBox    = document.getElementById('error-box');
  const resultCard  = document.getElementById('result-card');
  const hintEmpty   = document.getElementById('hint-empty');

  const todayStr = new Date().toISOString().split('T')[0];
  dateInput.max = todayStr;

  // NASA APOD архивът реално започва на 16.06.1995 — преди това няма данни
  const MIN_DATE = '1995-06-16';
  dateInput.min = MIN_DATE;

  // винаги стартираме с 01.03.2008
  const defaultDate = '2008-03-01';
  dateInput.value = defaultDate;

  // тук пазим последната успешно преведена дестинация (по подразбиране 01.03.2008),
  // за да имаме към какво да се "върнем", ако преводачът спре да работи
  let cachedDefaultView = null;

  // --- превод EN -> BG чрез MyMemory API ---
  // MyMemory приема максимум ~500 символа на заявка, затова режем текста
  // на изречения и превеждаме на парчета, после ги съединяваме.
  function splitIntoChunks(text, maxLen = 450) {
    const sentences = text.match(/[^.!?]+[.!?]*/g) || [text];
    const chunks = [];
    let current = '';
    for (const sentence of sentences) {
      if ((current + sentence).length > maxLen && current) {
        chunks.push(current.trim());
        current = sentence;
      } else {
        current += sentence;
      }
    }
    if (current.trim()) chunks.push(current.trim());
    return chunks;
  }

  // MyMemory не хвърля HTTP грешка, когато лимитът свърши — връща статус 200,
  // но вместо превод пише "MYMEMORY WARNING: YOU USED ALL AVAILABLE..." в текста.
  // Затова трябва сами да проверим за тази фраза.
  function isQuotaExceeded(text) {
    return typeof text === 'string' && text.toUpperCase().includes('MYMEMORY WARNING');
  }

  async function translateToBulgarian(text) {
    const chunks = splitIntoChunks(text);
    const translated = [];
    for (const chunk of chunks) {
      const res = await fetch(`https://api.mymemory.translated.net/get?q=${encodeURIComponent(chunk)}&langpair=en|bg`);
      const data = await res.json();
      const piece = data?.responseData?.translatedText || chunk;

      if (isQuotaExceeded(piece)) {
        throw new Error('MYMEMORY_QUOTA_EXCEEDED');
      }
      translated.push(piece);
    }
    return translated.join(' ');
  }

  // извиква се, когато NASA API-то откаже (лимит, мрежа, невалиден ключ и т.н.)
  function handleNasaFailure() {
    if (cachedDefaultView) {
      // имаме запазена дестинация (01.03.2008) — връщаме се към нея вместо да чупим страницата
      dateInput.value = defaultDate;
      document.getElementById('destination-img').src = cachedDefaultView.img;
      document.getElementById('destination-title').textContent = cachedDefaultView.title;
      document.getElementById('modal-title').textContent = cachedDefaultView.title;
      document.getElementById('destination-date').textContent = cachedDefaultView.date;
      document.getElementById('destination-desc').textContent = cachedDefaultView.desc;

      errorBox.textContent = 'NASA API достигна лимита си за заявки. Показваме запазената дестинация (01.03.2008).';
      errorBox.hidden = false;
      hintEmpty.hidden = true;
      resultCard.hidden = false;
    } else {
      // нямаме дори резервна дестинация (провалило се е и самото първо зареждане)
      errorBox.textContent = 'Заявката до НАСА се провали. Провери връзката или API ключа.';
      errorBox.hidden = false;
      resultCard.hidden = true;
    }
  }

  async function loadDestination(chosenDate) {
    if (!chosenDate || chosenDate > todayStr) {
      errorBox.textContent = 'Газовата уредба на машината на времето гръмна. Моля, изберете минала дата!';
      errorBox.hidden = false;
      resultCard.hidden = true;
      hintEmpty.hidden = true;
      return;
    }
    if (chosenDate < MIN_DATE) {
      errorBox.textContent = 'Голфът не е тунингован чак толкова назад — НАСА няма снимки отпреди 16.06.1995 г. Изберете по-скорошна дата!';
      errorBox.hidden = false;
      resultCard.hidden = true;
      hintEmpty.hidden = true;
      return;
    }
    errorBox.hidden = true;

    let data;
    try {
      const res = await fetch(`https://api.nasa.gov/planetary/apod?api_key=${NASA_API_KEY}&date=${chosenDate}`);
      data = await res.json();
    } catch (err) {
      // мрежата/заявката към НАСА се счупи изцяло
      handleNasaFailure();
      return;
    }

    // NASA връща грешка като { error: { code: "...", message: "..." } },
    // например при OVER_RATE_LIMIT — трябва да проверим и това, не само data.code
    if (data.error || data.code || !data.url) {
      handleNasaFailure();
      return;
    }

    document.getElementById('destination-img').src = data.media_type === 'image' ? data.url : (data.thumbnail_url || '');
    document.getElementById('destination-date').textContent = data.date;
    document.getElementById('destination-title').textContent = 'Превежда се...';
    document.getElementById('modal-title').textContent = 'Превежда се...';
    document.getElementById('destination-desc').textContent = 'Превежда се...';
    hintEmpty.hidden = true;
    resultCard.hidden = false;

    // кешираме снимката на дестинацията по подразбиране веднага —
    // не чакаме превода, за да имаме към какво да се върнем дори ако само НАСА API-то откаже някой друг път
    if (chosenDate === defaultDate) {
      cachedDefaultView = {
        img: document.getElementById('destination-img').src,
        title: data.title,
        date: data.date,
        desc: data.explanation
      };
    }

    // преводът е в собствен try/catch — ако MyMemory откаже, не искаме да се показва боклук
    try {
      const [titleBg, descBg] = await Promise.all([
        translateToBulgarian(data.title),
        translateToBulgarian(data.explanation)
      ]);

      document.getElementById('destination-title').textContent = titleBg;
      document.getElementById('modal-title').textContent = titleBg;
      document.getElementById('destination-desc').textContent = descBg;

      // ако това е дестинацията по подразбиране — обновяваме кеша с преведения текст
      if (chosenDate === defaultDate && cachedDefaultView) {
        cachedDefaultView.title = titleBg;
        cachedDefaultView.desc = descBg;
      }

    } catch (err) {
      if (err.message === 'MYMEMORY_QUOTA_EXCEEDED' && cachedDefaultView) {
        // лимитът на преводача е свършил — връщаме се към последната валидна
        // дестинация (01.03.2008), вместо да показваме "MYMEMORY WARNING..."
        dateInput.value = defaultDate;
        document.getElementById('destination-img').src = cachedDefaultView.img;
        document.getElementById('destination-title').textContent = cachedDefaultView.title;
        document.getElementById('modal-title').textContent = cachedDefaultView.title;
        document.getElementById('destination-date').textContent = cachedDefaultView.date;
        document.getElementById('destination-desc').textContent = cachedDefaultView.desc;

        errorBox.textContent = 'Преводачът изчерпа безплатния си лимит за днес. Показваме запазената дестинация (01.03.2008).';
        errorBox.hidden = false;
      } else {
        // друг проблем с превода (не лимит) — просто показваме оригиналния английски текст
        document.getElementById('destination-title').textContent = data.title;
        document.getElementById('modal-title').textContent = data.title;
        document.getElementById('destination-desc').textContent = data.explanation;
      }
    }
  }

  form.addEventListener('submit', function (e) {
    e.preventDefault();
    loadDestination(dateInput.value);
  });

  // зареждаме дестинацията по подразбиране веднага при отваряне на страницата
  loadDestination(defaultDate);