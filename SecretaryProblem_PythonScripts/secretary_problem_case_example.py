import mlagents
import numpy as np
from mlagents_envs.environment import UnityEnvironment as UE                                    # 유니티 환경 Load
from mlagents_envs.environment import ActionTuple 
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel  # 유니티 환경 엔진 설정 관리 클래스 (timescale 조절)
from torch.utils.tensorboard import SummaryWriter

# Unity 환경 로드
engine_configuration_channel = EngineConfigurationChannel()   # 유니티 엔진의 timescale을 조절할 채널
env = UE(file_name='./SecretaryProblem_UnityEnv_Build/Case_Example/SecretaryProblem_UnityEnv', seed=1, side_channels=[engine_configuration_channel])
env.reset()

# 유니티 브레인 설정 behavior (학습시킬 agent type) 불러오기
behavior_name = list(env.behavior_specs.keys())[0] # "Case_1?team=0"
spec = env.behavior_specs[behavior_name]
engine_configuration_channel.set_configuration_parameters(time_scale=1.0)  # 학습이 빨리 진행 될 수 있도록 10배속
decision_steps, terminal_steps = env.get_steps(behavior_name)               # 각각 decision을 request한 step정보, terminate된 step 정보           


for episode in range(1000):
  env.reset()
  
  # 에이전트가 행동을 요청한 상태인지, 마지막 상태(terminal state)인지 확인
  decision_steps, terminal_steps = env.get_steps(behavior_name)
  
  # 한 에이전트를 기준으로 로그를 출력
  tracked_agent = -1
  done = False
  ep_rewards = 0
  
  # 본격적인 하나의 episode
  while not done:
    # 추적할 agent 지정
    if tracked_agent == -1 and len(decision_steps) >= 1:
      tracked_agent = decision_steps.agent_id[0]
    
    # 랜덤 액션 결정
    action = ActionTuple(discrete=np.array([[1]], dtype=np.int32))
    
    # set actions
    env.set_actions(behavior_name, action)
    
    # 실제 액션 수행
    env.step()
    
    # 스탭 종료 후 에이전트의 정보 (보상, 상태) 확인
    decision_steps, terminal_steps = env.get_steps(behavior_name)
    
    # state 상태별로 나누어 보상 저장
    if tracked_agent in decision_steps:
      ep_rewards += decision_steps[tracked_agent].reward
    if tracked_agent in terminal_steps:
      ep_rewards += terminal_steps[tracked_agent].reward
      done = True
      
  # 해당 에피소드에서의 보상 출력
  print(f'total reward for ep {episode} is {ep_rewards}')
  
# 환경 종료
env.close()