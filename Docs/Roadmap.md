# Gameplay basics

- [x] Two users can play in one session
- [x] User can make actions at his turn
- [x] User can complete his turn (switch to other player turn)
- [x] First turn - to random player
- [x] User has global, hand and table card sets
- [x] User has resources, its max count is increased every turn and restored
- [x] Every turn user gets one card from global set to hand set (if hand set is not full)
- [x] Card can has a price for usage
- [x] Card type: creature, usage - is sent to table set (with place selection)
- [x] Table set is limited
- [x] Hand set is limited and invisible to other player
- [x] Global sets are invisible to any player
- [x] Creature has damage and health
- [x] User has health
- [x] Every turn creature can attack other player's creature or player (except first turn after summon)
- [x] If creature loses all its health, it is removed from table set
- [x] If player loses all his health, other player wins
- [x] Two players can play using server

# Basic UX

- [x] Card animations (get, buy, attack)
- [x] Dialogs (error, end, lost connection)
- [x] Mock game art
- [x] Handle lost connection

# Next UX

- [ ] Many anims: damage, destroy
- [ ] Login, sessions UI

# Basic web admin app

- [x] Ability to manage users in separated web app

# Web admin improvements

- [ ] Bootstrap design
- [ ] Login validation
- [ ] Error handling

# Security

- [x] Client/server: Password => hash(pass+salt(login))
- [ ] Admin: Setup password input
- [ ] Admin: Connection config and trusted hosts list

# Basic AI

- [ ] Buy, attack
- [ ] Select action with better weight
- [ ] Small randomization