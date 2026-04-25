# Poradnik Designera: System Questów i Dialogów

Ten dokument wyjaśnia, jak dodawać do gry nowe zadania (Questy), pisać dialogi, tworzyć nieliniowe rozmowy z wyborami, konfigurować udźwiękowienie oraz podpinać je pod postacie w świecie gry. 

Cały system opiera się na **Scriptable Objects (SO)**. Oznacza to, że każde zadanie i każdy fragment rozmowy to po prostu plik w folderze projektu, który konfigurujesz w Inspektorze Unity.

---

## 1. System Zadań (Questy)

Zadania w grze tworzą łańcuchy zdarzeń. Dodanie nowego zadania wymaga stworzenia pliku i zarejestrowania go w głównym systemie.

### Jak stworzyć nowy Quest?
1. W oknie **Project** przejdź do folderu, w którym trzymamy dane (np. `Assets/Data/Quests`).
2. Kliknij **Prawy Przycisk Myszy (PPM)** -> **Create** -> **System Questów** -> **Quest**.
3. Nadaj plikowi czytelną nazwę (np. `ZnajdzKlucz`).
4. Kliknij na nowy plik i spójrz w okno **Inspector**.

<img width="959" height="733" alt="image" src="https://github.com/user-attachments/assets/ddab94fb-70af-46ad-9938-b6157d13c131" />

### Wyjaśnienie Pól w Inspektorze:
* **Title:** Nazwa zadania widoczna w dzienniku (np. "Tajemnica piwnicy").
* **Short Description:** Opis zadania widoczny dla gracza. System na ekranie HUD zawsze wyświetla powiadomienie o najnowszym odblokowanym, ale jeszcze nieukończonym zadaniu.
* **Previous Quest:** *Bardzo ważne pole!* Jeśli chcesz, aby to zadanie odblokowało się **automatycznie** po ukończeniu innego, przeciągnij tutaj plik poprzedniego zadania. Jeśli zadanie odblokowuje się przez dialog lub wejście w strefę, zostaw to puste.
* **Hide Info When Locked:** Jeśli zaznaczone (True), gracz zobaczy w dzienniku zadanie jako "???", dopóki go nie odblokuje. Używaj tego do ukrywania spoilerów fabularnych.

<img width="508" height="311" alt="image" src="https://github.com/user-attachments/assets/2003ebb7-b232-4160-8bf3-ecffe2f7f476" />

### Krok krytyczny: Rejestracja Questa!
Samo stworzenie pliku nie sprawi, że gra go zobaczy. 
Po stworzeniu pliku `QuestSO`, musisz odnaleźć na scenie obiekt **QuestManager** i dodać swój nowy plik do listy **All Quests**. Inaczej dziennik nie pokaże Twojego zadania!
<img width="499" height="673" alt="image" src="https://github.com/user-attachments/assets/3d503755-7dda-4d04-a9a0-d4eb14c12817" />

---

## 2. System Dialogów

Dialogi pozwalają na tworzenie rozmów, wyborów i przeplatanie narracji z questami. Każdy plik dialogu to jeden "węzeł" (node) rozmowy.

### Jak stworzyć nowy Dialog?
1. Przejdź do folderu `Assets/Data/Dialogues`.
2. **PPM** -> **Create** -> **Dialogi** -> **Dialog**.
3. Nadaj nazwę (np. `Szef_Powitanie`).

<img width="907" height="714" alt="image" src="https://github.com/user-attachments/assets/6a9ece27-2113-4a2d-99a0-96d12aa8ba41" />

### Wyjaśnienie Pól w Inspektorze:
#### Ustawienia Rozmowy (Header)
* **Can Walk Away:**
  * Zaznaczone (True): Gracz może przerwać rozmowę po prostu odchodząc od postaci (używaj do mało ważnych plotek NPC).
  * Odznaczone (False): Gracz zostaje "zamrożony" w miejscu i musi dokończyć rozmowę (używaj do ważnych cutscenek i kluczowych NPC).
* **Typing Speed:** Szybkość pojawiania się tekstu. Domyślnie `0.03`. Zmniejsz wartość (np. do `0.01`), by postać "mówiła" szybciej. *Wskazówka: Gracz może wcisnąć przycisk interakcji w trakcie pisania, by pominąć animację i wyświetlić od razu cały tekst.*
<img width="501" height="177" alt="image" src="https://github.com/user-attachments/assets/ef7b789e-af79-4eee-af3d-55c00f8cf760" />

#### Linie Dialogowe (Lines)
Rozwiń listę `Lines` i dodaj nowy element (przycisk `+`). Każdy element to jeden "dymek" tekstu:
* **Speaker Name:** Imię postaci wyświetlane nad tekstem (np. "Szef", "???").
* **Speaker Icon:** Obrazek/Portret postaci (Sprite). Zostaw puste, jeśli postać nie ma portretu.
* **Text:** Treść wypowiedzi.
* **Dubbing Clip:** Długi plik dźwiękowy (`AudioClip`) z pełnym dubbingiem linii. Będzie odtwarzany w trybie 2D (niezależnie od dystansu). Zostanie natychmiast przerwany, jeśli gracz pominie pisanie tekstu.
* **Babble Container:** Zasób typu `Audio Random Container` (natywny system Unity). Odpowiada za "mruczenie" (babbling) postaci podczas pojawiania się liter. Odtwarzany lokalnie w trybie 3D. *Aby poprawnie działał, wejdź w ustawienia kontenera audio, zmień Trigger na `Automatic`, a Mode na `Pulse` (z czasem np. 0.06s).*
<img width="492" height="521" alt="image" src="https://github.com/user-attachments/assets/30ad097c-cd33-472d-9604-5e38f7190c62" />

#### Wybory (Choices)
Po wyświetleniu wszystkich Linii, gracz może otrzymać opcje wyboru. Jeśli zostawisz tę listę pustą, okno dialogowe po prostu się zamknie. Jeśli chcesz dać wybór, rozwiń `Choices` i dodaj element:
* **Choice Text:** Co będzie napisane na przycisku dla gracza (np. "Zgadzam się", "Muszę iść").
* **Next Dialogue:** Plik `DialogueSO`, który załaduje się po kliknięciu tej opcji. **W ten sposób tworzysz drzewka dialogowe!** Zostaw puste, jeśli wybór ma po prostu zakończyć rozmowę.
* **Quest To Unlock/Complete:** Plik `QuestSO`, który zostanie automatycznie wręczony/zaliczony graczowi po wybraniu tej opcji. Zostaw puste, jeśli wybór nie daje/nie zalicza żadnego questa.
<img width="496" height="268" alt="image" src="https://github.com/user-attachments/assets/3d3787c6-7b90-42c5-91bd-0f0317228cbc" />

---

## 3. Jak podpiąć dialog pod postać na scenie?

Kiedy masz już gotowe pliki i drzewko rozmowy, musisz przypisać je do modelu 3D na scenie gry.

1. Wybierz obiekt w grze (np. model strażnika), z którym gracz ma porozmawiać.
2. Upewnij się, że obiekt ma standardowy **Collider** (np. `BoxCollider`).
3. Jeśli postać nie ma mieć dubbingu, tylko babbling, dodaj do obiektu komponent **Audio Source**. W jego ustawieniach zmień `Spatial Blend` całkowicie na 3D (wartość 1) oraz dostosuj `Min/Max Distance` w `3D Sound Settings`. Będzie to służyło do odtwarzania babblingu. W przypadku dubbingu pomiń ten krok.
4. Dodaj do obiektu komponent (skrypt): **Dialogue Trigger**.
5. W polu **Main Dialogue** przeciągnij swój plik `DialogueSO` (ten, od którego rozmowa ma się rozpocząć).
6. W polu **Local Audio Source** przypisz komponent Audio Source, który dodałeś w kroku trzecim.
7. Obiekt *musi* znajdować się na warstwie, która pozwala na interakcję (najczęściej: BLUE). Zostanie automatycznie podświetlony, gdy gracz do niego podejdzie, a tekst interakcji zmieni się na "Rozmawiaj" lub "Kontynuuj" w zależności od stanu.

---

## 4. Dialogi w tle (Idle / Barks)

Jeśli chcesz, aby postać rzucała luźne teksty widoczne nad jej głową (bez bezpośredniej interakcji gracza), użyj dedykowanego systemu Idle.

1. Przygotuj normalny plik `DialogueSO` z tekstami (zignoruj w nim pole Wyborów).
2. Podepnij pod postać na scenie obiekt typu **Canvas** (zmieniony na tryb *World Space*) z komponentem **TextMeshPro - Text**.
3. Dodaj do postaci skrypt **Idle Dialogue Controller**.
4. Przypisz w skrypcie swój plik `DialogueSO`, Canvas, obiekt Tekstu oraz lokalny `Audio Source` postaci.
5. Postać będzie w nieskończoność zapętlać tekst nad swoją głową, odtwarzając dubbing/babbling. Jeśli postać ma również przypisany `Dialogue Trigger`, system wyłączy teksty tła na czas właściwej rozmowy, a potem automatycznie je przywróci.

---

## Przykładowy Workflow (Najlepsze praktyki)

Chcesz stworzyć sytuację, w której postać daje Ci zadanie dopiero, gdy będziesz dla niej niemiły?
1. Stwórz zadanie: `Quest_Zemsta`. Nie dodawaj mu `Previous Quest`. (Pamiętaj o dodaniu go do QuestManagera!).
2. Stwórz dialog "zły": `Dialog_Niemily`. W nim postać się złości.
3. Stwórz dialog bazowy: `Dialog_Poczatek`. Postać mówi "Witaj".
4. W `Dialog_Poczatek` w sekcji **Choices** dodaj opcję: "Jesteś głupi!".
5. W tej opcji jako **Next Dialogue** ustaw `Dialog_Niemily`.
6. W tej samej opcji jako **Quest To Unlock** ustaw `Quest_Zemsta`.
7. Dodaj `DialogueTrigger` oraz `Audio Source` do modelu na scenie i przypisz mu `Dialog_Poczatek`.

Gotowe! Stworzyłeś nieliniową narrację rozdającą zadania ze zintegrowanym systemem dźwięku przestrzennego, nie pisząc ani jednej linijki kodu.

---

## 5. Znaczniki Formatowania (TextMeshPro)

System wykorzystuje tagi zbliżone do HTML (np. `<b>tekst</b>`). Tagi można zagnieżdżać, pamiętając o ich zamykaniu w odwróconej kolejności (np. `<b><color="red">tekst</color></b>`). 

Oto kompletne zestawienie znaczników obsługiwanych przez system TextMeshPro:

### Style i Transformacja
* **`<b>...</b>`** – Pogrubienie tekstu.
* **`<i>...</i>`** – Kursywa.
* **`<u>...</u>`** – Podkreślenie.
* **`<s>...</s>`** – Przekreślenie.
* **`<lowercase>...</lowercase>`** – Wymusza małe litery.
* **`<uppercase>...</uppercase>`** – Wymusza wielkie litery.
* **`<smallcaps>...</smallcaps>`** – Wyświetla tekst jako kapitaliki (małe wielkie litery).

### Kolor i Widoczność
* **`<color="nazwa/HEX">...</color>`** – Zmienia kolor (np. `red`, `blue`, `yellow` lub `<color=#FF0000>`).
* **`<alpha=#FF>`** – Ustawia przezroczystość tekstu w skali szesnastkowej (od `00` do `FF`). Zmiana dotyczy całego tekstu po tagu.
* **`<mark=#FFFF0080>...</mark>`** – Podświetla tło pod tekstem (działa jak marker).

### Rozmiar i Skalowanie
* **`<size=X>...</size>`** – Skaluje tekst (wartość absolutna np. `14`, lub procentowa np. `150%`).
* **`<sub>...</sub>`** – Indeks dolny (np. H<sub>2</sub>O).
* **`<sup>...</sup>`** – Indeks górny (np. m<sup>2</sup>).

### Pozycjonowanie i Spacing
* **`<align=X>...</align>`** – Wyrównanie w poziomie (`left`, `right`, `center`, `justified`, `flush`).
* **`<line-height=X>...</line-height>`** – Odstępy między wierszami (np. `150%`).
* **`<cspace=X>...</cspace>`** – Zmienia odstępy między znakami (kerning, np. `1em`).
* **`<space=X>`** – Wstawia puste miejsce w linii o określonej szerokości.
* **`<margin=X>...</margin>`** – Określa marginesy boczne akapitu (np. `10%`).
* **`<indent=X>...</indent>`** – Dodaje wcięcie tylko do pierwszego wiersza akapitu.
* **`<pos=X>`** – Przesuwa karetkę do konkretnej pozycji poziomej w bieżącej linii (np. `50%`).
* **`<voffset=X>...</voffset>`** – Przesuwa dany fragment tekstu w pionie (np. `1em` lub `-1em`).

### Zasoby i Funkcje Specjalne
* **`<sprite=X>`** lub **`<sprite name="X">`** – Wstawia obrazek/ikonkę z przypisanego zasobu *Sprite Asset*.
* **`<font="Nazwa">...</font>`** – Zmienia czcionkę na inną (plik musi znajdować się w folderze *Resources*).
* **`<link="ID">...</link>`** – Tworzy interaktywny fragment, na który można kliknąć (wymaga logiki w kodzie C#).
* **`<nobr>...</nobr>`** – Wiąże słowa ze sobą, zapobiegając przełamaniu linii w połowie frazy.
* **`<noparse>...</noparse>`** – Ignoruje inne tagi wewnątrz, traktując je jako zwykły tekst (przydatne np. do pokazywania kodu).
* **`<page>`** – Sztucznie dzieli blok tekstu na strony (przydatne w listach lub książkach).
