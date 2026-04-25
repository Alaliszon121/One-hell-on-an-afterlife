# Wykrywanie intencji odpowiedzi gracza
Czyli reagowanie na dowolny tekst wpisany przez gracza przy użyciu silnika dopasowania intencji opartego na wagach słów kluczowych.

---

## 1. Podstawowa Mechanika: tzw. Weighted Keyword Scoring
System nie próbuje "zrozumieć" pełnej gramatyki zdania. Zamiast tego skanuje tekst gracza w poszukiwaniu **słów kluczowych**, które przypisane są do konkretnych **intencji**.

1.  **Input:** Gracz wpisuje tekst (np. *"I really loved that movie, it was amazing!"*).
2.  **Scoring:** Silnik sprawdza, ile słów kluczowych z każdej intencji znajduje się w tekście.
3.  **Winning Intent:** Intencja, która zdobędzie najwięcej punktów (wag), zostaje wybrana.
4.  **Reaction:** NPC odsyła wiadomość przypisaną do wygranej intencji.

---

## 2. Przykładowa Baza

### 2.1 
**Pytanie NPC:** *"What do you think about workers' rights?"* 

| Intent | Keywords | Weight | Match Type | Blokady negacji | Reaction Pool |
| :--- | :--- | :---: | :--- | :--- | :--- |
| **Pro_Corporate**<br>*(Gracz zgadza się z NPC, pogardza prawami)* | `overrat`, `useless` | **+3** | Częściowe | `not`, `aren't` | "Exactly! If they wanted rights, they should have just been born rich. It's basic biology." / "Finally, someone gets it! Empathy is just a tax on productivity." / "Right? I'm firing my assistant tomorrow just to feel something." |
| | `laz`, `entitl` | **+2** | Częściowe | `not`, `don't` | |
| | `robot`, `ai` | **+3** | Częściowe | `not`, `aren't` | |
| | `profit`, `replace` | **+2** | Częściowe | `don't` | |
| **Pro_Worker**<br>*(Gracz broni praw, moralizuje)* | `union`, `strike` | **+3** | Częściowe | `anti`, `against` | "Ew. Are you in a union? Let me step back, I'm severely allergic to the middle class." / "Please keep your voice down. My yacht requires the tears of the working class to float." / "Are you crying? HR will deduct this emotional outburst from your paycheck." |
| | `fair`, `human` | **+2** | Częściowe | `not`, `don't` | |
| | `wage`, `pay` | **+2** | Dokładne | `don't` | |
| | `deserv` | **+2** | Częściowe | `don't`, `not` | |
| **Apathy**<br>*(Gracz ma to gdzieś / Chce tylko kasy)* | `don't care`, `dont care` | **+4** | Częściowe | Brak | "That's the spirit! Ignorance is bliss, and highly profitable for me." / "Good. Thinking is for people who can't afford to hire others to think for them." / "Excellent mindset. You'll make a great middle-manager one day." |
| | `whatever`, `idk` | **+3** | Częściowe | Brak | |
| | `money`, `cash` | **+1** | Częściowe | `no` | |
| **[Fallback]**<br>*(Wynik = 0)* | *(Brak dopasowań)* | **0** | Brak | Brak | "Did you just quote a socialist manifesto at me? I only speak the language of tax evasion." / "I have no idea what you just said, so I'm just going to assume you're fired." |


### 2.2 Jak system rozwiąże skomplikowane i podchwytliwe odpowiedzi?

**Sytuacja 1: Próba obrony pracowników używając "ich" słów**
Gracz wpisuje: *"They are **not robot**s, they are **human**s and **deserv**e **fair** **pay**!"*
* `not robot` (Pro_Corporate +3... **ZABLOKOWANE** przez prefiks `not`. Wynik = 0).
* `human` (Pro_Worker: +2).
* `deserv` (Pro_Worker: +2).
* `fair` (Pro_Worker: +2).
* `pay` (Pro_Worker: +2).
* **Wynik:** Pro_Worker miażdży inne intencje (8 punktów do 0). Gra perfekcyjnie omija pułapkę słowa "robot" dzięki blokadzie, a NPC reaguje obrzydzeniem na empatię gracza ("Ew. Are you in a union?").

**Sytuacja 2: Mroczny humor i zgodność z NPC**
Gracz wpisuje: *"They are just **laz**y, **replace** them with **AI** to keep the **profit**."*
* `laz` (Pro_Corporate: +2).
* `replace` (Pro_Corporate: +2).
* `ai` (Pro_Corporate: +3).
* `profit` (Pro_Corporate: +2).
* **Wynik:** Pro_Corporate wygrywa (9 punktów). NPC odpisuje: *"Exactly! If they wanted rights, they should have just been born rich."*

**Sytuacja 3: Odwrócenie logiki (Negacja po stronie pracowniczej)**
Gracz wpisuje: *"I'm **against union**s, workers are so **entitl**ed."*
* `against union` (Pro_Worker +3... **ZABLOKOWANE** przez prefiks `against`. Wynik = 0).
* `entitl` (Pro_Corporate: +2).
* **Wynik:** Pro_Corporate wygrywa (2 punkty do 0). Gracz skutecznie zaprzeczył byciu "za związkami", więc system poprawnie to zignorował i skupił się na słowie krytykującym postawę roszczeniową (`entitl` od *entitled*).

---

## 3. Workflow w silniku Unity
System oparty jest na **Scriptable Objects**.

### Krok 1: Tworzenie Profilu NPC
1.  W oknie *Project* kliknij prawym przyciskiem: `Create > Dialogi > Profil NPC`.
2.  Nazwij plik imieniem postaci (np. `Ania_Profil`).
3.  W Inspektorze wpisz `Imie NPC`.

### Krok 2: Dodawanie Kontekstu (Pytania)
1.  Wewnątrz profilu NPC dodaj nowy element do listy `Questions Contexts`.
2.  `Context ID`: Unikalna nazwa (np. `movie_discussion`).
3.  `NPC Question Text`: Wpisz pytanie, które NPC wyśle do gracza.

### Krok 3: Definiowanie Intencji i Słów Kluczowych
1.  Wewnątrz Kontekstu dodaj `Possible Intents` (np. 3 elementy: Positive, Negative, Neutral).
2.  Dla każdej intencji dodaj `Keywords`:
    * Wpisz słowo (np. `amaz`).
    * Ustaw `Weight` (domyślnie 1 lub 2).
    * Zaznacz `Exact Match Only` tylko jeśli słowo nie powinno być częścią innych słów (np. dla słowa "no").

### Krok 4: Pisanie Reakcji
1.  W sekcji `NPC Reactions` danej intencji dodaj odpowiedź/odpowiedzi.
2.  Jeśli dodasz więcej niż jedną, system automatycznie wylosuje jeden z nich.

---

## 4. Dobre praktyki dla Designera
* NPC powinien reagować krótkimi, naturalnymi komunikatami SMS (1-3 zdania).
* Używaj rdzeni słów (np. `excit` zamiast `excited`), aby system był bardziej odporny na literówki i różne formy gramatyczne gracza.
* Nadawaj wyższą wagę (+3) słowom unikalnym dla danej intencji (np. `masterpiece` dla pochwały), a niższą (+1) słowom pospolitym (np. `yes`).
* Zawsze przygotuj 2-3 generyczne odpowiedzi typu "Nie rozumiem", które pasują do charakteru danego NPC.
