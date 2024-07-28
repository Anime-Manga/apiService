import Logger from "../applications/logger";
import fastify from "../applications/serverFastify";

const logger = new Logger("anime-controller");

logger.debug("Loaded controller!")

fastify.get('/', async function handler (request, reply) {
  return { hello: 'world' }
})

fastify.post('/', async function handler (request, reply) {
  return { hello: 'world' }
})

export default fastify;