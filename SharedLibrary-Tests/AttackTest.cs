using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;
using Xunit;

namespace SharedLibrary_Tests {
    public class AttackTest {
		[Fact]
		public void CreaturesWithoutHealthIsRemoved() {
			var state = Common.GameState;
			var user  = state.Users[0];
			var card  = user.HandSet.Find(
				c => (c.Type == CardType.Creature) && (user.Power >= c.Price)
			);
			var buyAction = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			buyAction.Apply(state);
			Assert.True(user.TableSet.Contains(card));
			card.Health = 0;
			new EmptyAction().Apply(state);
			Assert.False(user.TableSet.Contains(card));
		}

		GameState CreatePlayerAttackReadyState() {
			var state = Common.GameState;
			var user  = state.Users[0];
			var card  = user.HandSet.Find(
				c => (c.Type == CardType.Creature) && (user.Power >= c.Price)
			);
			var buyAction = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			buyAction.Apply(state);
			card.Actions = card.MaxActions;
			return state;
		}

		[Fact]
		public void PlayerLostHealth() {
			var state = CreatePlayerAttackReadyState();
			var action = new AttackPlayerAction("1", 0);
			Assert.True(action.CanApply(state));
			action.Apply(state);
			Assert.True(state.Users[1].Health == state.Users[1].MaxHealth - state.Users[0].TableSet[0].Damage);
		}

		[Fact]
		public void PlayerAttackSpentActions() {
			var state = CreatePlayerAttackReadyState();
			var action = new AttackPlayerAction("1", 0);
			Assert.True(action.CanApply(state));
			action.Apply(state);
			var card = state.Users[0].TableSet[0];
			Assert.True(card.Actions < card.MaxActions);
		}

		[Fact]
		public void CantAttackPlayerWithoutCreature() {
			var state = CreatePlayerAttackReadyState();
			var action = new AttackPlayerAction("1", -1);
			Assert.False(action.CanApply(state));
			action = new AttackPlayerAction("1", 1);
			Assert.False(action.CanApply(state));
		}

		[Fact]
		public void CantAttackPlayerByInactiveCreature() {
			var state = CreatePlayerAttackReadyState();
			state.Users[0].TableSet[0].Actions = 0;
			var action = new AttackPlayerAction("1", 0);
			Assert.False(action.CanApply(state));
		}

		GameState CreateCreatureAttackReadyState() {
			var state = CreatePlayerAttackReadyState();
			new TurnAction("1").Apply(state);
			var user = state.Users[1];
			var card = user.HandSet.Find(
				c => (c.Type == CardType.Creature) && (user.Power >= c.Price)
			);
			var buyAction = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			buyAction.Apply(state);
			new TurnAction("2").Apply(state);
			return state;
		}

		[Fact]
		public void CreatureLostHealth() {
			var state = CreateCreatureAttackReadyState();
			var userCard = state.Users[0].TableSet[0];
			var enemyCard = state.Users[1].TableSet[0];
			enemyCard.Health = enemyCard.MaxHealth = userCard.Damage + 1;
			var action = new AttackCreatureAction("1", 0, 0);
			Assert.True(action.CanApply(state));
			action.Apply(state);
			Assert.True(enemyCard.Health == enemyCard.MaxHealth - userCard.Damage);
		}

		[Fact]
		public void CreatureAttackSpentActions() {
			var state = CreateCreatureAttackReadyState();
			var userCard = state.Users[0].TableSet[0];
			var action = new AttackCreatureAction("1", 0, 0);
			Assert.True(action.CanApply(state));
			action.Apply(state);
			Assert.True(userCard.Actions < userCard.MaxActions);
		}

		[Fact]
		public void CantAttackCreatureWithoutCreature() {
			var state = CreateCreatureAttackReadyState();
			var action = new AttackCreatureAction("1", -1, 0);
			Assert.False(action.CanApply(state));
			action = new AttackCreatureAction("1", 1, 0);
			Assert.False(action.CanApply(state));
		}

		[Fact]
		public void CantAttackNotExistingCreatureCreature() {
			var state = CreateCreatureAttackReadyState();
			var action = new AttackCreatureAction("1", 0, -1);
			Assert.False(action.CanApply(state));
			action = new AttackCreatureAction("1", 0, 1);
			Assert.False(action.CanApply(state));
		}

		[Fact]
		public void CantAttackCreatueByInactiveCreature() {
			var state = CreateCreatureAttackReadyState();
			var userCard = state.Users[0].TableSet[0];
			userCard.Actions = 0;
			var action = new AttackCreatureAction("1", 0, 0);
			Assert.False(action.CanApply(state));
		}
	}
}
