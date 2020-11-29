:-dynamic connect/2.

cargar(A):-exists_file(A),consult(A).

newConnect(Num1, Num2) :-
	 assert(connect(Num1, Num2)).

exist(X,Y):-connect(X,_),
	Y is 0.

success(_).

addingConnect([],_,_).
addingConnect([H|T],P,D):-
	addingConnect(T,P,D),
	assert(connect(H,P)),
	retractall(connect(H,D)).

addConnect(Num1,NewNum,X):-
	connect(NewNum,D),
	findall(L,connect(L, D),LDelete),
	connect(Num1, P),
	D \= P,
	addingConnect(LDelete,P,D),
	findall(M,connect(M,D),NL),
	retractall(connect(_,D)),
	call_delete_repeat(NL,NL2),
	cleanGroups(NL2,D),
	success(X),!.


addConnect(Num1,NewNum,X) :- connect(Num1, X),
	assert(connect(NewNum, X)),!,
	findall(M,connect(M,X),NL),
	retractall(connect(_,X)),
	call_delete_repeat(NL,NL2),
	cleanGroups(NL2,X).

getConections(X,Y,P):- connect(X,P),
	findall(F,connect(F,P),Nlist),
	retractall(connect(_,P)),
	call_delete_repeat(Nlist,N2list),!,
	cleanGroups(N2list,P),
	connect(Y,P).

cleanGroups([],_).
cleanGroups([H|T],N):-
	assert(connect(H,N)),
	cleanGroups(T,N).

conections(X,Y):-connect(X,Y).


getTotalForGroup(Group,Quantity):-
         findall(P,connect(P,Group),ListGroup),
	 length(ListGroup,Quantity).

getGroups(List):-
	findall(P,connect(_,P),List).



delete_repeat(_,[],[]):-!.
delete_repeat(X,[X|Xs],Zs):-
	delete_repeat(X,Xs,Zs).
delete_repeat(X,[Y|Xs],[Y|Zs]):-
	X \== Y,
	delete_repeat(X,Xs,Zs).

call_delete_repeat([], []):-!.

call_delete_repeat([H|T], [H|T1]) :-
       delete_repeat(H, T, T2),
       call_delete_repeat(T2, T1).


getAllGroups([], []).
getAllGroups([H|T], [List|T1]):-
	findall(P,connect(P,H),List),
	getAllGroups(T,T1).


getListAllGroups(NLSubLists):-
	getGroups(LGroups),
	call_delete_repeat(LGroups,NewLGroups),
	getAllGroups(NewLGroups,NLSubLists).

getLenghtLists([],[]).
getLenghtLists([H|T],[Size|T1]):-
	length(H,Size),
	getLenghtLists(T,T1).

getSizesGroups(List):-
	getListAllGroups(L),
	getLenghtLists(L,List).

getListGroups(Nlist):-
	getGroups(List),
	call_delete_repeat(List,Nlist).

clean(Numb1,Numb2):-retractall(connect(Numb1,Numb2)).
