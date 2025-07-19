use rapier2d::prelude::*;

struct World {
    query: QueryPipeline,
    bodies: RigidBodySet,
    colliders: ColliderSet
}

static mut WORLD: Option<World> = None;

#[no_mangle]
pub extern "C" fn bench_rapier() -> i32 {
    let mut physics = PhysicsPipeline::new();

    let mut world = World{
        query: QueryPipeline::new(),
        bodies: RigidBodySet::new(),
        colliders: ColliderSet::new()
    };
    
    for y in -8..8 {
        for x in -8..8 {
            let body = RigidBodyBuilder::dynamic().translation(vector!(x as f32,y as f32)).build();
            let bh = world.bodies.insert(body);
        
            let collider = ColliderBuilder::new(SharedShape::ball(0.5)).build();
            world.colliders.insert_with_parent(collider,bh,&mut world.bodies);
        }
    }

    {
        let collider = ColliderBuilder::new(SharedShape::cuboid(20.0,1.0)).translation(vector!(0.0,-20.0)).build();
        world.colliders.insert(collider);
    }
    {
        let collider = ColliderBuilder::new(SharedShape::cuboid(1.0,20.0)).translation(vector!(20.0,0.0)).build();
        world.colliders.insert(collider);
    }
    {
        let collider = ColliderBuilder::new(SharedShape::cuboid(1.0,20.0)).translation(vector!(-20.0,0.0)).build();
        world.colliders.insert(collider);
    }

    let ip = IntegrationParameters::default();
    let mut islands = IslandManager::new();
    let mut broad_phase = BroadPhaseMultiSap::new();
    let mut narrow_phase = NarrowPhase::new();
    let mut j1 = ImpulseJointSet::new();
    let mut j2 = MultibodyJointSet::new();
    let mut ccd = CCDSolver::new();

    for i in 0..200 {
        physics.step(&vector![0.0,-10.0], &ip, &mut islands, &mut broad_phase, &mut narrow_phase,
            &mut world.bodies, &mut world.colliders, &mut j1, &mut j2, &mut ccd, Some(&mut world.query), &(), &());
    }

    unsafe { WORLD = Some(world); }
    0
}

#[no_mangle]
pub extern "C" fn physics_test(x: f32, y: f32) -> i32 {
    unsafe {
        let world = WORLD.as_ref().unwrap();

        let mut count = 0;
    
        world.query.intersections_with_point(&world.bodies, &world.colliders, &point!(x,y), QueryFilter::default(), |c|{
            count += 1;
            true
        });
    
        count
    }
}
