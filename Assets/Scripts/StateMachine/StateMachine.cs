using System;
using UnityEngine;
using System.Collections.Generic;

public class StateMachine
{
    StateNode currentState;
    Dictionary<Type, StateNode> nodes = new();
    public string CurrentStateName => currentState?.GetType().Name ?? "None";
    HashSet<ITransition> anyTransitions = new();
    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
        {
            ChangeState(transition.To);
        }
        currentState.State?.Update();
    }
    public void FixedUpdate()
    {
        currentState.State?.FixedUpdate();
    }
    public void SetState(IState state)
    {
        currentState = nodes[state.GetType()];
        currentState.State.EnterState();
    }
    public void ChangeState(IState state)
    {
        if (currentState.State == state) return;
        var previous = currentState.State;
        var next = nodes[state.GetType()].State;
        previous?.Exit();
        next?.EnterState();
        currentState = nodes[state.GetType()];

    }
    public ITransition GetTransition()
    {
        foreach (var transition in anyTransitions)
        {
            if (transition.Condition.Evaluate()) return transition;
        }
        foreach (var transition in currentState.Transitions)
        {
            if (transition.Condition.Evaluate()) return transition;
        }
        return null;
    }
    public void AddTransition(IState From, IState To, IPredicate condition)
    {
        GetOrAddNode(From).AddTransition(GetOrAddNode(To).State, condition);
    }
    public void AddAnyTransition(IState To, IPredicate condition)
    {
        anyTransitions.Add(new Transition(GetOrAddNode(To).State, condition));
    }
    StateNode GetOrAddNode(IState state)
    {
        var node = nodes.GetValueOrDefault(state.GetType());
        if (node == null)
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }
        return node;
    }


    class StateNode
    {
        public IState State { get; }
        public HashSet<ITransition> Transitions { get; }
        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<ITransition>();
        }
        public void AddTransition(IState To, IPredicate condition)
        {
            Transitions.Add(new Transition(To, condition));
        }
    }
}