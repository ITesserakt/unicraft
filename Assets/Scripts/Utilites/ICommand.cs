
using System;

namespace mc2.utils {
	public interface ICommand {
	}

	public interface IObservableCommand<in TObservable> : ICommand {
		void Execute(IObservable<TObservable> obs);
	}

	public interface IUpdatableCommand : IObservableCommand<long> {
		
	}
}