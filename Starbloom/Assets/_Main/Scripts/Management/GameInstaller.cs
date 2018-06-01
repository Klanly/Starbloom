using UnityEngine;
using Zenject;
using WorldTime;

public class GameInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.Bind<ITimeEventHandler>().To<DefaultTimeEventHandler>().AsSingle().NonLazy();
		Container.DeclareSignal<OnMinuteSignal>().NonLazy();
		Container.DeclareSignal<OnHourSignal>().NonLazy();
		Container.DeclareSignal<OnDaySignal>().NonLazy();
		Container.DeclareSignal<OnMonthSignal>().NonLazy();
		Container.DeclareSignal<OnYearSignal>().NonLazy();
	}
}