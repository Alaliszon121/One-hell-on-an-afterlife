# Poradnik Designera: System Questów i Dialogów

Ten dokument wyjaśnia, jak dodawać do gry nowe zadania (Questy), pisać dialogi, tworzyć nieliniowe rozmowy z wyborami oraz podpinać je pod postacie w świecie gry. 

Cały system opiera się na **Scriptable Objects (SO)**. Oznacza to, że każde zadanie i każdy fragment rozmowy to po prostu plik w folderze projektu, który konfigurujesz w Inspektorze Unity.

---

## 1. System Zadań (Questy)

Zadania w grze tworzą łańcuchy zdarzeń. Dodanie nowego zadania wymaga stworzenia pliku i zarejestrowania go w głównym systemie.

### Jak stworzyć nowy Quest?
1. W oknie **Project** przejdź do folderu, w którym trzymamy dane (np. `Assets/Data/Quests`).
2. Kliknij **Prawy Przycisk Myszy (PPM)** -> **Create** -> **System Questów** -> **Quest**.
3. Nadaj plikowi czytelną nazwę (np. `Q01_ZnajdzKlucz`).
4. Kliknij na nowy plik i spójrz w okno **Inspector**.

### Wyjaśnienie Pól w Inspektorze:
* **Title:** Nazwa zadania widoczna w dzienniku (np. "Tajemnica piwnicy").
* **Short Description:** Opis zadania widoczny dla gracza.
* **Previous Quest:** *Bardzo ważne pole!* Jeśli chcesz, aby to zadanie odblokowało się **automatycznie** po ukończeniu innego, przeciągnij tutaj plik poprzedniego zadania. Jeśli zadanie odblokowuje się przez dialog lub wejście w strefę, zostaw to puste.
* **Hide Info When Locked:** Jeśli zaznaczone (True), gracz zobaczy w dzienniku zadanie jako "???", dopóki go nie odblokuje. Używaj tego do ukrywania spoilerów fabularnych.

### Krok krytyczny: Rejestracja Questa!
Samo stworzenie pliku nie sprawi, że gra go zobaczy. 
Po stworzeniu pliku `QuestSO`, musisz odnaleźć na scenie obiekt **QuestManager** i dodać swój nowy plik do listy **All Quests**. Inaczej dziennik nie pokaże Twojego zadania!

---

## 2. System Dialogów

Dialogi pozwalają na tworzenie kinowych rozmów, wyborów moralnych oraz płynne przeplatanie narracji z questami. Każdy plik dialogu to jeden "węzeł" (node) rozmowy.

### Jak stworzyć nowy Dialog?
1. Przejdź do folderu `Assets/Data/Dialogues`.
2. **PPM** -> **Create** -> **Dialogi** -> **Dialog**.
3. Nadaj nazwę (np. `Szef_Powitanie`).

### Wyjaśnienie Pól w Inspektorze:
#### Ustawienia Rozmowy (Header)
* **Can Walk Away:**
  * Zaznaczone (True): Gracz może przerwać rozmowę po prostu odchodząc od postaci (używaj do mało ważnych plotek NPC).
  * Odznaczone (False): Gracz zostaje "zamrożony" w miejscu i musi dokończyć rozmowę (używaj do ważnych cutscenek i kluczowych NPC).
* **Typing Speed:** Szybkość pojawiania się tekstu. Domyślnie `0.03`. Zmniejsz wartość (np. do `0.01`), by postać "mówiła" szybciej.

#### Linie Dialogowe (Lines)
Rozwiń listę `Lines` i dodaj nowy element (przycisk `+`). Każdy element to jeden "dymek" tekstu:
* **Speaker Name:** Imię postaci wyświetlane nad tekstem (np. "Szef", "???").
* **Speaker Icon:** Obrazek/Portret postaci (Sprite). Zostaw puste, jeśli postać nie ma portretu.
* **Text:** Treść wypowiedzi.
* **Voice Clip:** Krótki dźwięk (AudioClip) odtwarzany na początku wyświetlania tekstu (np. stęknięcie, przywitanie lub pełny dubbing).

#### Wybory (Choices)
Po wyświetleniu wszystkich Linii, gracz może otrzymać opcje wyboru. Jeśli zostawisz tę listę pustą, okno dialogowe po prostu się zamknie. Jeśli chcesz dać wybór, rozwiń `Choices` i dodaj element:
* **Choice Text:** Co będzie napisane na przycisku dla gracza (np. "Zgadzam się", "Muszę iść").
* **Next Dialogue:** Plik `DialogueSO`, który załaduje się po kliknięciu tej opcji. **W ten sposób tworzysz drzewka dialogowe!** Zostaw puste, jeśli wybór ma po prostu zakończyć rozmowę.
* **Quest To Unlock/Complete:** Plik `QuestSO`, który zostanie automatycznie wręczony/zaliczony graczowi po wybraniu tej opcji. Zostaw puste, jeśli wybór nie daje/nie zalicza żadnego questa.

---

## 3. Jak podpiąć dialog pod postać na scenie?

Kiedy masz już gotowe pliki i drzewko rozmowy, musisz przypisać je do modelu 3D na scenie gry.

1. Wybierz obiekt w grze (np. model strażnika), z którym gracz ma porozmawiać.
2. Upewnij się, że obiekt ma standardowy **Collider** (np. `BoxCollider`).
3. Dodaj do obiektu komponent (skrypt): **Dialogue Trigger**.
4. W polu **Dialogue** w tym skrypcie, przeciągnij swój plik `DialogueSO` (ten, od którego rozmowa ma się rozpocząć).
5. Obiekt *musi* znajdować się na warstwie, która pozwala na interakcję (najczęściej: BLUE). Zostanie automatycznie podświetlony, gdy gracz do niego podejdzie, a tekst interakcji zmieni się na "Rozmawiaj" lub "Kontynuuj" w zależności od stanu.

---

## Przykładowy Workflow (Najlepsze praktyki)

Chcesz stworzyć sytuację, w której postać daje Ci zadanie dopiero, gdy będziesz dla niej niemiły?
1. Stwórz zadanie: `Quest_Zemsta`. Nie dodawaj mu `Previous Quest`. (Pamiętaj o dodaniu go do QuestManagera!).
2. Stwórz dialog "zły": `Dialog_Niemily`. W nim postać się złości.
3. Stwórz dialog bazowy: `Dialog_Poczatek`. Postać mówi "Witaj".
4. W `Dialog_Poczatek` w sekcji **Choices** dodaj opcję: "Jesteś głupi!".
5. W tej opcji jako **Next Dialogue** ustaw `Dialog_Niemily`.
6. W tej samej opcji jako **Quest To Unlock** ustaw `Quest_Zemsta`.
7. Dodaj `DialogueTrigger` do modelu na scenie i przypisz mu `Dialog_Poczatek`.

Gotowe! Stworzyłeś nieliniową narrację rozdającą zadania, nie pisząc ani jednej linijki kodu.
