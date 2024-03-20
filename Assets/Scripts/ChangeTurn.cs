using Mirror;

public class ChangeTurn : NetworkBehaviour
{
    [SyncVar] //Переменные типа SyncVar могут изменяться только на сервере, а не на клиенте
    NetworkIdentity IdentityOfDepartingPlayer; //Создаем переменную, в которой будет храниться иднтефикатор игрока, который только что сходил
}
