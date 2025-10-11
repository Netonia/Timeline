# Product Requirements Document (PRD)  
## Project: Timeline of People – Blazor WASM App

---

### 1. Overview
The **Timeline of People** is a **single-page Blazor WebAssembly application** that displays a chronological visual of notable individuals, including their names, birth and death dates, and short descriptions.  
Example use case: a timeline of famous scientists involved in physics and quantum mechanics.

---

### 2. Goals
- Display people on a horizontal or vertical timeline ordered by date of birth.  
- Show key information: name, birth–death years, and description.  
- Allow users to add, edit, or remove entries locally.  
- Provide filtering or search (e.g., by name or century).  

---

### 3. Non-Goals
- No authentication or user profiles.  
- No external databases or APIs.  
- No image uploads or multimedia features.  

---

### 4. Target Users
- Students studying history or science.  
- Educators creating visual aids.  
- Hobbyists visualizing historical data.  

---

### 5. Core Features
#### 5.1 Input Form
- Fields:  
  - Name (string)  
  - Date of Birth (YYYY-MM-DD)  
  - Date of Death (YYYY-MM-DD or “Alive”)  
  - Description (short text)  
- Button to **add person** to the timeline.  
- Optional: inline edit and delete.  

#### 5.2 Timeline Display
- Rendered as a **chronological timeline** (horizontal on desktop, vertical on mobile).  
- Each entry shows:  
  - Name  
  - Life span (birth–death)  
  - Description  
- Hover or click expands details.  
- Optional: highlight selected or hovered entry.

#### 5.3 Data Handling
- Data stored in memory; optional persistence via localStorage.  
- Load sample dataset on first run (e.g., famous physicists).

---

### 6. Technical Requirements
- **Framework:** Blazor WebAssembly (standalone).  
- **Libraries:**  
  - Optional: timeline visualization library (custom SVG or CSS-based layout).  
- **Storage:** browser localStorage.  
- **Hosting:** GitHub Pages or static hosting.  

---

### 7. UI/UX Layout
| Section | Description |
|----------|--------------|
| Header | App title: “Timeline of People” |
| Left/Top | Form to add/edit person entries |
| Main Area | Scrollable timeline visualization |
| Footer | Dataset info and optional export/import |

---

### 8. Security
- Fully client-side.  
- No data transmission.  

---

### 9. Success Metrics
- Timeline sorts correctly by birth date.  
- Entries display readable life spans.  
- Add/Edit/Delete works locally.  
- Responsive layout on desktop and mobile.  

---

### 10. Example Dataset
| Name | Born | Died | Description |
|------|------|------|-------------|
| Isaac Newton | 1643 | 1727 | Laws of motion and universal gravitation |
| Albert Einstein | 1879 | 1955 | Theory of relativity |
| Niels Bohr | 1885 | 1962 | Atomic structure and quantum theory |
| Richard Feynman | 1918 | 1988 | Quantum electrodynamics |
| Erwin Schrödinger | 1887 | 1961 | Wave mechanics |

---

### 11. Example Flow
1. User opens app → sees sample timeline of physicists.  
2. User adds new entry: “Marie Curie, 1867–1934, Discovered polonium and radium.”  
3. Timeline updates in real time.  
4. User searches “Einstein” → timeline scrolls to entry.  

---

### 12. Future Enhancements
- Import/export JSON dataset.  
- Category tagging (e.g., “Physics,” “Mathematics”).  
- Timeline zoom and filtering by century.  
- Optional photo or portrait for each person.  
- Animated transitions between entries.
