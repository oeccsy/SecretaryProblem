import mlagents
import numpy as np
from mlagents_envs.environment import UnityEnvironment as UE                                    # 유니티 환경 Load
from mlagents_envs.environment import ActionTuple 
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel  # 유니티 환경 엔진 설정 관리 클래스 (timescale 조절)
# from torch.utils.tensorboard import SummaryWriter
import random
import math

import numpy as np
np.bool = bool

# Unity 환경 로드
engine_configuration_channel = EngineConfigurationChannel()   # 유니티 엔진의 timescale을 조절할 채널
env = UE(file_name='./SecretaryProblem_UnityEnv_Build/Case_MonteCarlo/SecretaryProblem_UnityEnv', seed=1, side_channels=[engine_configuration_channel])
env.reset()

# 유니티 브레인 설정 behavior (학습시킬 agent type) 불러오기
behavior_name = list(env.behavior_specs.keys())[0] # "Case_1?team=0"
spec = env.behavior_specs[behavior_name]
engine_configuration_channel.set_configuration_parameters(time_scale=10.0)  # 학습이 빨리 진행 될 수 있도록 10배속
decision_steps, terminal_steps = env.get_steps(behavior_name)               # 각각 decision을 request한 step정보, terminate된 step 정보           

# monte carlo control을 위한 settings
Q = np.zeros((10,10,2)) # (현재 면접자 순서, 현재 면접자의 지금까지 중 순위, action - pass or select)
alpha = 0.01
epsilon = 0.9

def epsilon_greedy():
  
  rand = np.random.random()
  if rand < epsilon:
    action = random.randint(0,1)
  else:
    order = int(decision_steps.obs[0][0][1])
    ranking = int(decision_steps.obs[0][0][2])
    
    action_val = Q[order-1,ranking-1,:]
    action = np.argmax(action_val)
    
  return action

def update_agent(history):
  cum_reward = 0
  for transition in history[::-1]:
    order, ranking, a, r = transition
    
    # 몬테카를로 방식으로 업데이트
    Q[order-1, ranking-1, a] = Q[order-1, ranking-1, a] + alpha * (cum_reward - Q[order-1, ranking-1, a])
    cum_reward = cum_reward + r
    
def anneal_eps():
  global epsilon
  epsilon -= 0.00003
  epsilon = max(epsilon, 0.1)

def show_Q():
  q_list = Q.tolist()
  data = np.zeros((10,10))
  for row_idx in range(len(q_list)):
    row = q_list[row_idx]
    for col_idx in range(len(row)):
      col = row[col_idx]
      action = np.argmax(col)
      data[row_idx, col_idx] = action
    
  print(data)  

# number of rounds
num_rounds = 100000

for episode in range(num_rounds):
  env.reset()
  
  # 에이전트가 행동을 요청한 상태인지, 마지막 상태(terminal state)인지 확인
  decision_steps, terminal_steps = env.get_steps(behavior_name)
  
  # 한 에이전트를 기준으로 로그를 출력
  tracked_agent = -1
  done = False
  history = []
  
  # 본격적인 하나의 episode
  while not done:
    # 추적할 agent 지정
    if tracked_agent == -1 and len(decision_steps) >= 1:
      tracked_agent = decision_steps.agent_id[0]
    
    # states 확인
    order = int(decision_steps.obs[0][0][1])
    ranking = int(decision_steps.obs[0][0][2])
    
    # 액션 결정
    action = epsilon_greedy()
    action_for_settings = ActionTuple(discrete=np.array([[action]], dtype=np.int32))
    
    # set actions
    env.set_actions(behavior_name, action_for_settings)
    
    # 실제 액션 수행
    env.step()
    
    # 스탭 종료 후 에이전트의 정보 (보상, 상태) 확인
    decision_steps, terminal_steps = env.get_steps(behavior_name)
    
    # state 상태별로 나누어 보상 저장
    if tracked_agent in decision_steps:
      history.append((order, ranking, action, decision_steps[tracked_agent].reward)) 
    if tracked_agent in terminal_steps:
      history.append((order, ranking, action, terminal_steps[tracked_agent].reward)) 
      done = True
    
    print(f'episode {episode} is done')
    # 해당 에피소드에서의 history로 Agent 업데이트
    
  update_agent(history)
  anneal_eps()
  
# 환경 종료
show_Q()
env.close()