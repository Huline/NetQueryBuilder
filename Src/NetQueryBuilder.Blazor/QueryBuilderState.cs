// using Microsoft.Extensions.DependencyInjection;
// using NetQueryBuilder.Conditions;
// using NetQueryBuilder.Configurations;
// using NetQueryBuilder.Queries;
//
// namespace NetQueryBuilder.Blazor;
//
// public interface IQueryStateProvider
// {
//     IQueryFacade<TEntity> GetOrCreate<TEntity>(string stateId = "default") where TEntity : class;
//     void Remove<TEntity>(string stateId = "default") where TEntity : class;
//     bool Exists<TEntity>(string stateId = "default") where TEntity : class;
// }
//
// public class QueryStateProvider : IQueryStateProvider, IDisposable
// {
//     private readonly IServiceProvider _serviceProvider;
//     private readonly Dictionary<string, object> _states = new();
//     private readonly Dictionary<string, IDisposable> _disposables = new();
//     
//     public QueryStateProvider(IServiceProvider serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//     }
//     
//     public IQueryFacade<TEntity> GetOrCreate<TEntity>(string stateId = "default") where TEntity : class
//     {
//         var key = $"{typeof(TEntity).FullName}:{stateId}";
//         
//         if (_states.TryGetValue(key, out var existing))
//         {
//             return (IQueryFacade<TEntity>)existing;
//         }
//         
//         // Créer un nouveau QueryActor
//         var actor = ActivatorUtilities.CreateInstance<QueryActor<TEntity>>(_serviceProvider);
//         
//         // Créer la façade
//         var facade = new QueryFacade<TEntity>(actor);
//         
//         // Stocker l'état
//         _states[key] = facade;
//         
//         // Stocker les disposables pour pouvoir les libérer plus tard
//         _disposables[key] = actor;
//         
//         return facade;
//     }
//     
//     public void Remove<TEntity>(string stateId = "default") where TEntity : class
//     {
//         var key = $"{typeof(TEntity).FullName}:{stateId}";
//         
//         if (_states.TryGetValue(key, out _))
//         {
//             _states.Remove(key);
//             
//             if (_disposables.TryGetValue(key, out var disposable))
//             {
//                 disposable.Dispose();
//                 _disposables.Remove(key);
//             }
//         }
//     }
//     
//     public bool Exists<TEntity>(string stateId = "default") where TEntity : class
//     {
//         var key = $"{typeof(TEntity).FullName}:{stateId}";
//         return _states.ContainsKey(key);
//     }
//     
//     public void Dispose()
//     {
//         foreach (var disposable in _disposables.Values)
//         {
//             disposable.Dispose();
//         }
//         
//         _states.Clear();
//         _disposables.Clear();
//     }
// }
//
// public interface IQueryFacade<TEntity> where TEntity : class
// {
//     IObservable<QueryState<TEntity>> State { get; }
//     
//     // API simplifiée pour les composants
//     Task Initialize();
//     Task UpdateSelectedFields(IEnumerable<SelectPropertyPath> selectedFields);
//     Task UpdateCondition(ICondition condition);
//     Task ExecuteQuery();
//     Task Reset();
// }
//
// public class QueryState<T> where T : class
// {
//     public Query<T> Query { get; }
//     public QueryResultState<T> Result { get; }
// }
//
// public class QueryResultState<T> where T : class
// {
//     public int TotalItems { get; }
//     public int CurrentPage { get; }
//     public int TotalPages { get; }
//     public IEnumerable<T> Items { get; }
// }
//
// public class QueryFacade<TEntity> : IQueryFacade<TEntity>, IDisposable where TEntity : class
// {
//     private readonly IQueryActor<TEntity> _actor;
//     
//     public QueryFacade(IQueryActor<TEntity> actor)
//     {
//         _actor = actor;
//     }
//     
//     public IObservable<QueryState<TEntity>> State => _actor.StateObservable;
//     
//     public Task Initialize() => _actor.SendAsync(new InitializeQuery<TEntity>());
//     
//     public Task UpdateSelectedFields(IEnumerable<SelectPropertyPath> selectedFields) => 
//         _actor.SendAsync(new UpdateSelectedFields<TEntity>(selectedFields));
//     
//     public Task UpdateCondition(ICondition condition) => 
//         _actor.SendAsync(new UpdateCondition(condition));
//     
//     public Task ExecuteQuery() => 
//         _actor.SendAsync(new ExecuteQuery());
//     
//     public Task Reset() => 
//         _actor.SendAsync(new ResetQuery());
//     
//     public void Dispose()
//     {
//         // Rien à faire ici si l'acteur est géré par DI
//     }
// }
//
//
//
//
//
//
// public class QueryBuilderState
// {
//     public EventHandler<QueryChangedEventArgs>? QueryChanged;
//     public EventHandler<QueryExecutedEventArgs>? QueryExecuted;
//     
//     public Task ExecuteQueryAsync()
//     {
//         QueryExecuted?.Invoke(this, new QueryExecutedEventArgs());
//         return Task.CompletedTask;
//     }
// }
//
// public class QueryChangedEventArgs : EventArgs
// {
// }
// public class QueryExecutedEventArgs : EventArgs
// {
// }
//
//
// public interface IQueryActor<TEntity> where TEntity : class
// {
//     // Observable pour s'abonner aux changements d'état
//     IObservable<QueryState<TEntity>> StateObservable { get; }
//     
//     // Méthodes pour envoyer des messages à l'acteur
//     Task SendAsync(QueryMessage message);
// }
//
// public class QueryActor<TEntity> : IQueryActor<TEntity>, IDisposable where TEntity : class
// {
//     private readonly IQueryConfigurator _queryConfigurator;
//     private readonly Subject<QueryMessage> _incomingMessages = new();
//     private readonly BehaviorSubject<QueryState<TEntity>> _stateSubject;
//     private readonly IDisposable _messageSubscription;
//     
//     public QueryActor(IQueryConfigurator queryConfigurator)
//     {
//         _queryConfigurator = queryConfigurator;
//         
//         // État initial
//         var initialQuery = _queryConfigurator.BuildFor<TEntity>();
//         var initialState = new QueryState<TEntity>
//         {
//             Query = initialQuery,
//             SelectedPropertyPaths = initialQuery.SelectPropertyPaths.ToList(),
//             Results = new List<TEntity>(),
//             TotalItems = 0,
//             IsLoading = false
//         };
//         
//         _stateSubject = new BehaviorSubject<QueryState<TEntity>>(initialState);
//         
//         // S'abonner aux messages
//         _messageSubscription = _incomingMessages
//             .ObserveOn(TaskPoolScheduler.Default)  // Pour le traitement asynchrone
//             .Subscribe(HandleMessage);
//     }
//     
//     public IObservable<QueryState<TEntity>> StateObservable => _stateSubject.AsObservable();
//     
//     public Task SendAsync(QueryMessage message)
//     {
//         _incomingMessages.OnNext(message);
//         return Task.CompletedTask;
//     }
//     
//     private async void HandleMessage(QueryMessage message)
//     {
//         var currentState = _stateSubject.Value;
//         
//         try
//         {
//             switch (message)
//             {
//                 case InitializeQuery<TEntity>:
//                     HandleInitialize();
//                     break;
//                 
//                 case UpdateSelectedFields<TEntity> updateFields:
//                     HandleUpdateSelectedFields(updateFields.SelectedFields);
//                     break;
//                 
//                 case UpdateCondition updateCondition:
//                     HandleUpdateCondition(updateCondition.Condition);
//                     break;
//                 
//                 case ExecuteQuery:
//                     await HandleExecuteQuery();
//                     break;
//                 
//                 case ResetQuery:
//                     HandleReset();
//                     break;
//             }
//         }
//         catch (Exception ex)
//         {
//             // Publier un événement d'erreur
//             _incomingMessages.OnNext(new QueryExecutionFailed(ex));
//         }
//     }
//     
//     private void HandleInitialize()
//     {
//         // Similaire au Reset mais pour l'initialisation
//         HandleReset();
//     }
//     
//     private void HandleUpdateSelectedFields(IEnumerable<SelectPropertyPath> selectedFields)
//     {
//         var current = _stateSubject.Value;
//         var updatedState = current.With(selectedPropertyPaths: selectedFields.ToList());
//         _stateSubject.OnNext(updatedState);
//     }
//     
//     private void HandleUpdateCondition(ICondition condition)
//     {
//         var current = _stateSubject.Value;
//         var query = current.Query;
//         query.Condition = condition;
//         var updatedState = current.With(query: query);
//         _stateSubject.OnNext(updatedState);
//     }
//     
//     private async Task HandleExecuteQuery()
//     {
//         var current = _stateSubject.Value;
//         
//         // Mettre à jour l'état pour indiquer le chargement
//         _stateSubject.OnNext(current.With(isLoading: true));
//         
//         try
//         {
//             // Exécuter la requête
//             var result = await current.Query.Execute();
//             var data = (result as IEnumerable<TEntity>)?.ToList() ?? new List<TEntity>();
//             
//             // Mettre à jour l'état avec les résultats
//             var updatedState = current.With(
//                 results: data,
//                 totalItems: data.Count,
//                 isLoading: false
//             );
//             _stateSubject.OnNext(updatedState);
//             
//             // Publier l'événement de requête exécutée
//             _incomingMessages.OnNext(new QueryExecuted<TEntity>(data, data.Count));
//         }
//         catch (Exception ex)
//         {
//             // Remettre l'état à non-chargement
//             _stateSubject.OnNext(current.With(isLoading: false));
//             
//             // Relancer pour un traitement en amont
//             throw ex;
//         }
//     }
//     
//     private void HandleReset()
//     {
//         var newQuery = _queryConfigurator.BuildFor<TEntity>();
//         newQuery.Condition.CreateNew(newQuery.ConditionPropertyPaths.First());
//         
//         var newState = new QueryState<TEntity>
//         {
//             Query = newQuery,
//             SelectedPropertyPaths = newQuery.SelectPropertyPaths.ToList(),
//             Results = new List<TEntity>(),
//             TotalItems = 0,
//             IsLoading = false
//         };
//         
//         _stateSubject.OnNext(newState);
//     }
//     
//     public void Dispose()
//     {
//         _messageSubscription?.Dispose();
//         _incomingMessages?.Dispose();
//         _stateSubject?.Dispose();
//     }
// }
//
