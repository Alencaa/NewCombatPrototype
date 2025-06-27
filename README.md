NewCombatPrototype

đây là cơ chế của game tôi đang muốn hướng tới, nó bao gồm như thế này:

\[START INPUT] ← Player initiates input (LMB / RMB)
===

# 

# │

# ├── Swipe LMB (Single) ─────────────┐

# │                                   │

# │    Normal Attack                  │

# │                                   ↓

# │                             \[CHECK COLLISION]

# │                                   │

# │                                   ↓

# ├── Hold LMB → Then Swipe ─────────▶ Heavy Attack

# │                                   │

# │                                   ↓

# ├── Hold RMB + Drag ───────────────▶ Directional Block (Hold)

# │                                   │

# ├── Quick Swipe RMB ───────────────▶ Parry Attempt (Active Block)

# │                                   │

# ├── FLUID GESTURE ATTACK ──────────▶ Check: 

# │                                   │

# │                                   ├── Valid gesture path? (Ex: Down → Up → Side)

# │                                   ├── Performed within combo time window?

# │                                   └── Has enough STAMINA?

# │                                         └── Yes → Execute combo hit (1 / 2 / 3...)

# │                                         └── No → Combo ends early (idle)

# 

# ────────────────────────────────────────────────────

# 

# \[COLLISION DETECTED: Player Attack ↔ Enemy]

# 

# │

# ├── Enemy is ATTACKING

# │     ├── Player hits first → Counter Hit (Extra damage)

# │     └── Enemy hits first → Player takes damage

# │

# ├── Enemy is BLOCKING (Directional)

# │     ├── Direction match:

# │     │     ├── If Parry Timing OK → Perfect Parry → Enemy stagger

# │     │     └── Else → Normal Block → No damage, Reduce Posture

# │     ├── Direction mismatch → Hit lands (enemy takes damage)

# │     └── If Player used Heavy → Extra posture damage

# │           └── If Posture ≤ 0 → Guard Break + Stagger

# │

# ├── Enemy is in FEINT

# │     ├── Player reacts too early → Whiff OR get countered

# │     └── Player times correctly → Damage enemy

# │

# └── Enemy is OPEN (Idle, Recovering)

# &nbsp;     └── Full Damage

# &nbsp;         └── If in combo → Chain continues

# 

# ────────────────────────────────────────────────────

# 

# \[ENEMY ACTION LOOP]

# 

# ├── Attack (Standard or Strong)

# ├── Block (Choose direction: Up / Side / Down)

# ├── Feint (Fake → delay → True attack)

# ├── Flurry (Multi-direction → Fast Combo)

# │     ├── Player must:

# │     │     ├── Hold Block + Drag to match direction

# │     │     └── Or Parry per-hit with Quick RMB swipe

# │     └── Wrong direction = Hit lands

# │

# └── Reaction to Player:

# &nbsp;     ├── If Player combos too long → Enemy parries back

# &nbsp;     ├── If Player heavy attack → Enemy posture check

# 

# ────────────────────────────────────────────────────

# 

# \[COMBO LOGIC – FLUID CHAIN SYSTEM]

# 

# ├── Chain = Gesture Path + Timing + Stamina

# │     ├── Valid input path (Ex: ↓→↑→→)

# │     ├── Delay between inputs < Combo Time Window

# │     ├── Each hit costs 1 Combo Stamina

# │

# ├── Break Combo if:

# │     ├── Gesture path invalid

# │     ├── Miss (no collision)

# │     ├── Hit gets blocked/parried

# │     ├── Stamina = 0

# │

# └── Finishers:

# &nbsp;     ├── If final gesture = heavy → Finisher hit

# &nbsp;     ├── On success → Big damage or Enemy stagger

# 

# ────────────────────────────────────────────────────

# 

# \[RESULT PHASE]

# 

# ├── Damage Calculation (HP / Posture / Stamina)

# ├── Trigger Effects (Particles, Camera, Sound)

# ├── Update FSM (AI / Player)

# └── Allow follow-up input or reset to neutral

# 

# ────────────────────────────────────────────────────

# 

# NOTE:

# \- STAMINA only affects Fluid Combo Chains

# \- Heavy \& Normal attack are always available if not interrupted

# \- Parry reward > Block, but risk higher

\- Block Hold = safer, but posture wears down over time

dưới đây là link dẫn tới những gì mà tôi muốn được thảo luận với chatGPT
https://chatgpt.com/share/685e3094-8980-800b-9d72-4c0a50e7c42b

link dẫn tới GGD trong notion:
https://www.notion.so/Game-Design-Document-21fcdc9ab4dd802b9d37eaa23cf9dc15?source=copy\_link

hiện tại tôi muốn update project này để làm theo những yêu cầu và thiết kế có trong này, hiện tôi đang dùng simple pixel sprite animation để prototype, tuy nhiên sau này tôi sẽ theo pineline 2D modular + rigging bằn spine (kết hợp vẽ bằng aseperite).
Hãy giúp tôi thiết kế phát triển một hệ thống thật scalable, thỏa mãn hết những design đã được đề cập trong này, có thể dựa trên những gì tôi đã xây dựng trong project này. Hoặc có thể làm lại từ đầu. Vì trong project tôi đi theo một hướng đi khác.
===



