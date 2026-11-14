import z from "zod";

const schemaVoid = z.object({
    status: z.literal("ok")
});
export type ISchemaVoid = z.infer<typeof schemaVoid>;